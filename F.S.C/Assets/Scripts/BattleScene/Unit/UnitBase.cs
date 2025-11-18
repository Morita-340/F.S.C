using System;
using System.Collections;
using System.Collections.Generic;
using FSCGeneral;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField]
    protected GSetting.ShapeType shapeType;
    [SerializeField,Range(1,100)]
    //現在のHP
    protected int HitPoint = 5;
    /// <summary>
    /// 生成時のHP
    /// </summary>
    [SerializeField,ReadOnly]
    protected int InstHitPoint = 0;
    [SerializeField,ReadOnly]
    protected MainCameraController MainCamera;
    [SerializeField,Range(-180f,180f)]
    protected float offsetRotation = 0;
    /// <summary>
    /// 素体のユニットであるかを判別する。インスペクターで予め設定する。プレイ中は一切変えない
    /// </summary>
    [SerializeField]
    protected bool isPrime = false;
    /// <summary>
    /// 被弾時にダメージ値を表示するオブジェクト
    /// </summary>
    [SerializeField]
    GameObject DamageCountText;
    protected SpriteRenderer spriteRenderer;
    protected GameObject thisGameObject;
    protected FieldManager FM = FieldManager.GetInstance();
    [SerializeField,ReadOnly]
    protected AbstractPartsController APC;
    protected RefineAbstractUnitDestroyManagementScript ReAUDMS;
    //カメラの拡大倍率の計算に用いるコアからの距離
    protected int distanseFromCore = 0;
    //通常攻撃力。コアや武器ユニットでは装着した武器の攻撃力が再代入される
    protected int normalAttackPower = 3;
    //チャージ攻撃力。コアや武器ユニットでは装着した武器の攻撃力が再代入される
    protected int chargeAttackPower = 0;
    //攻撃倍率。リアクターの影響を受けるとこの数字が増える
    [SerializeField,ReadOnly]
    protected int attackEfficiency = 1;
    /// <summary>
    /// 敵のユニットを破壊することで得られる経験値。PUDMSから代入するだけでこちらで計算することは無い
    /// </summary>
    protected int EXP = 0;
    private UnitBase thisUnit;
    //以下4つのUnitBase型変数は参照渡しにだけ利用すること。このデータを使いたい場合はUnitDataからアクセスすること。
    UnitBase upperUnit = null; 
    UnitBase downerUnit = null;
    UnitBase rightUnit = null; 
    UnitBase leftUnit = null;  
    protected UnitData ThisUnitData;
    private RectTransform uiRectTransform;
    [SerializeField,ReadOnly] protected SoundController SCer;
    public UnitData GetThisUnitData(){
        return ThisUnitData;
    }
    /// <summary>
    /// ユニット再生成処理時に設定するパラメータを関数にした。これにより関数の呼び出しでtagとlayerの設定を追うことが出来る
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="layerNum"></param>
    /// <returns></returns>
    public virtual GameObject UnitSetting(string tagName, int layerNum)
    {
        tag = tagName;
        this.gameObject.layer = layerNum;
        return this.gameObject;        
    }
    /// <summary>
    /// 攻撃倍率の変更
    /// </summary>
    /// <param name="reactorAttackEfficiency">
    /// リアクターの攻撃倍率の加減算処理。減算する時は負符号をつけること</param>
    protected virtual void AddAttackEfficiency(int reactorAttackEfficiency){
        float delayTime = 0f;
        //減算時＝リアクターが破壊された時は爆風で破壊されたユニットのステータスをスコアに加算しないといけないが、即時減算だとリアクターによる強化がスコアに反映されないので、体感では分からない程度に処理を遅らせる
        if(reactorAttackEfficiency < 0){delayTime = 0.2f;}
        if(gameObject.activeSelf)StartCoroutine(AttackEfficiencyAddDelay(delayTime,reactorAttackEfficiency));
    }
    IEnumerator AttackEfficiencyAddDelay(float delayTime,int reactorAttackEfficiency){
        yield return new WaitForSeconds(delayTime);
        attackEfficiency += reactorAttackEfficiency;
        //処理順序とかを間違えたとしても攻撃倍率が1未満にならないようにしたい
        if(attackEfficiency < 1){attackEfficiency = 1;}
    }
    /// <summary>
    /// ユニットの戦闘力を渡す
    /// </summary>
    /// <returns></returns>
    public virtual int GetUnitStatus(){
        return InstHitPoint ;
    }
    public GSetting.ShapeType GetShapeType(){
        return shapeType;
    }
    public AbstractPartsController GetAPC()
    {
        return APC;
    }
    private Vector3 RayBasePosition;//対応するPreviewObjectの座標を代入してある
    private float PreviewObjZRotate;//
    public void SetRayBasePosition(Vector3 position){
        RayBasePosition = position;
    }
    public void SetPreviewObjRotate(float ZRotate){
        PreviewObjZRotate = ZRotate;
    }
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        SCer = GetComponent<SoundController>();
        thisGameObject = this.gameObject;
        thisUnit = this;
        ThisUnitData = new UnitData(thisUnit, (int)shapeType, isPrime);
        FM.UnitList.Add(ThisUnitData);
        attackEfficiency = 1;
        if (this.tag == GSetting.ObjTagName.PlayerUnit.ToString()
        || this.tag == GSetting.ObjTagName.EnemyUnit.ToString()
        || this.tag == GSetting.ObjTagName.DestroyedUnit.ToString())//合体時は生成のタイミングではタグの変更を行えないのでこれを使う
        { distanseFromCore = CaluculateHowFarFromCore(this.transform.localPosition); }
        uiRectTransform = GameObject.Find("UICanvas").GetComponent<RectTransform>();
        if(this is JointUnit && tag == GSetting.ObjTagName.DestroyedUnit.ToString())GSetting.RefineDebugAssertinLog(transform,"aaaaa"+transform.localPosition.ToString()+"\n");
    }
    protected virtual void Start(){
        APC = thisGameObject.transform.parent.GetComponent<AbstractPartsController>();
        ReAUDMS = thisGameObject.transform.root.GetComponent<RefineAbstractUnitDestroyManagementScript>();
        if(isPrime){HitPoint = ReAUDMS.GetPrimeUnitsHP();}
        InstHitPoint = HitPoint;
        if(InstHitPoint <= 0){InstHitPoint = 1;}
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        MainCamera = GameObject.Find("Main Camera").GetComponent<MainCameraController>();
        GetAdjacentObjLink(tag);//RegistData()に格納するとなぜか動かなくなるので注意
    }
    protected virtual void GetAdjacentObjLink(string SelectedObjTag)
    {
        if (shapeType == GSetting.ShapeType.Square)
        {
            upperUnit = GetUpLink(SelectedObjTag);
            downerUnit = GetDownLink(SelectedObjTag);
            rightUnit = GetRightLink(SelectedObjTag);
            leftUnit = GetLeftLink(SelectedObjTag);
        }
        else if (shapeType == GSetting.ShapeType.RegularTriangle)
        {
            downerUnit = GetDownLink(SelectedObjTag);
        }
        else if (shapeType == GSetting.ShapeType.IsoscelesRightTriangle)
        {
            downerUnit = GetDownLink(SelectedObjTag);
            rightUnit = GetRightLink(SelectedObjTag);
        }
        else if (shapeType == GSetting.ShapeType.Rectangle)
        {
            rightUnit = GetRightLink(SelectedObjTag);
            leftUnit = GetLeftLink(SelectedObjTag);
        }
        else if (shapeType == GSetting.ShapeType.ObtusePentagon)
        {
            downerUnit = GetDownLink(SelectedObjTag);
            rightUnit = GetRightLink(SelectedObjTag);
            leftUnit = GetLeftLink(SelectedObjTag);
        }
        else { Debug.LogAssertion("ShapeType is null!"); }
        RegistData(upperUnit, downerUnit, rightUnit, leftUnit);
    }
    private void RegistData(UnitBase upperUnit,UnitBase downerUnit,UnitBase rightUnit,UnitBase leftUnit){
        ThisUnitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
    }
    private int CaluculateHowFarFromCore(Vector3 RegeneUnitsPosition){
        int distanseFromCore = (int)Math.Sqrt(Math.Pow(RegeneUnitsPosition.x,2)+Math.Pow(RegeneUnitsPosition.y,2));
        return distanseFromCore;
    }
    private UnitBase GetUpLink(string SelectedObjTag){
        UnitBase upperUnit;
        GameObject UpperObj = GetUpperGameObject(SelectedObjTag);
        if(UpperObj == null){return null;}
        upperUnit = UpperObj.GetComponent<UnitBase>();
        if(upperUnit == null)Debug.LogAssertion("UpperUnit is null;");
        return upperUnit;
    }
    private UnitBase GetDownLink(string SelectedObjTag){
        UnitBase downerUnit;
        GameObject DownerObj = GetDownerGameObject(SelectedObjTag);
        if (DownerObj == null) { return null; }
        downerUnit = DownerObj.GetComponent<UnitBase>();
        if(downerUnit == null)Debug.LogAssertion("DownerUnit is null;");
        return downerUnit;
    }
    private UnitBase GetRightLink(string SelectedObjTag){
        UnitBase rightUnit;
        GameObject RightObj = GetRightGameObject(SelectedObjTag);
        if(RightObj == null){return null;}
        rightUnit = RightObj.GetComponent<UnitBase>();
        if(rightUnit == null)Debug.LogAssertion("RightUnit is null;");
        return rightUnit;
    }
    private UnitBase GetLeftLink(string SelectedObjTag){
        UnitBase leftUnit;
        GameObject LeftObj = GetLeftGameObject(SelectedObjTag);
        if(LeftObj == null){return null;}
        leftUnit = LeftObj.GetComponent<UnitBase>();
        if(leftUnit == null)Debug.LogAssertion("LeftUnit is null;");
        return leftUnit;
    }
    private GameObject GetUpperGameObject(string SelectedObjTag){
        foreach(RaycastHit2D hit2D in RayCalculateAndCast(Vector3.up)){
            GameObject LeftObj = hit2D.collider.gameObject;
            if(LeftObj.tag == SelectedObjTag){return LeftObj;}
        }
        return null;
    }
    private GameObject GetDownerGameObject(string SelectedObjTag){
        foreach(RaycastHit2D hit2D in RayCalculateAndCast(Vector3.down)){
            GameObject LeftObj = hit2D.collider.gameObject;
            if (LeftObj.tag == SelectedObjTag) { return LeftObj; }
        }
        return null;
    }
    private GameObject GetRightGameObject(string SelectedObjTag){
        foreach(RaycastHit2D hit2D in RayCalculateAndCast(Vector3.right)){
            GameObject LeftObj = hit2D.collider.gameObject;
            if(LeftObj.tag == SelectedObjTag){return LeftObj;}
        }
        return null;
    }
    private GameObject GetLeftGameObject(string SelectedObjTag){
        foreach(RaycastHit2D hit2D in RayCalculateAndCast(Vector3.left)){
            GameObject LeftObj = hit2D.collider.gameObject;
            if(LeftObj.tag == SelectedObjTag){return LeftObj;}
        }
        return null;
    }
    /// <summary>
    /// オブジェクト種と調べたい隣のユニットの位置を入力にRayを照射する座標を計算している
    /// </summary>
    /// <param name="RayOffsetDirection">Rayを飛ばす位置をずらす際の初期値</param>
    /// <returns>座標を計算して照射したRayが当たったかどうかRaycastHit2Dで判定し、当たったなら情報を返している</returns>
    private RaycastHit2D[] RayCalculateAndCast(Vector3 RayOffsetDirection){
        Vector3 RayPosition = new Vector3(0,0,0);
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){//このオブジェクトのタグをenumでswitch文の分岐判別している
        //原点中心でオイラー角*ベクトルによる極座標を（直交座標系に変換して）Rayの本来の始点へ足し合わせて移動させている。この順番じゃないとちゃんと計算できないので注意
            case GSetting.ObjTagName.PlayerUnit://このオブジェクトの周辺のユニットを調べるためにRayを飛ばすので、このオブジェクトの座標と回転を考慮してオブジェクトから見た上下左右方向にRayを飛ばす
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.root.eulerAngles.z+this.gameObject.transform.localRotation.eulerAngles.z +offsetRotation) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
            case GSetting.ObjTagName.EnemyUnit://このオブジェクトの周辺のユニットを調べるためにRayを飛ばすので、このオブジェクトの座標と回転を考慮してオブジェクトから見た上下左右方向にRayを飛ばす
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.root.eulerAngles.z+this.gameObject.transform.localRotation.eulerAngles.z +offsetRotation) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
            case GSetting.ObjTagName.DestroyedUnit://鹵獲して接続する際に離れ小島になっていないかIsNotIsolatedUnit()で判別するときに使用される。値が違うだけで計算内容は上と一緒
                RayPosition = (Quaternion.Euler(0,0,this.gameObject.transform.root.eulerAngles.z+this.gameObject.transform.localRotation.eulerAngles.z +offsetRotation) * RayOffsetDirection) + this.gameObject.transform.position;
                break;
        }
        return Physics2D.RaycastAll(RayPosition,new Vector3(0,0,1));
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        //素体ユニットならReAUDMS側で共有しているHPに変更する
        if (isPrime) { HitPoint = ReAUDMS.GetPrimeUnitsHP(); }
        float HPRatio = (float)HitPoint/(float)InstHitPoint;
        spriteRenderer.color = new Color(HPRatio,HPRatio,HPRatio,spriteRenderer.color.a);
        //ReAUDMS?.ThisIsVisible(spriteRenderer.isVisible);
        if(HitPoint <= 0){DestroyUnit();}
    }
    public virtual IEnumerator NormalAttack(Vector3 TargetPosition){
        yield return null;
    }
    public virtual void ChargeAttack(Vector3 TargetPosition){}
    //HPが0になった時の破壊処理（分離の処理はReAUDMSが行う）
    protected virtual void DestroyUnit(){
        //リンク情報が消去されたのにゲームオブジェクトだけ消去されない場合の例外処理
        if(ThisUnitData == null){
            Debug.LogWarning("Exception Destroy Handle");
            Destroy(thisGameObject);
        }
        DivideUnitLink();
        //分離エフェクトを実装する
        Destroy(thisGameObject);
    }
    private void DivideUnitLink(){
        if(ThisUnitData != null){
            ThisUnitData.DeleteFourWayLink();
            FM.DeleteData(ThisUnitData);
            }
        if(APC != null)APC.DestroyProcess(ThisUnitData,false);
    }
    /// <summary>
    /// 離れ小島としてユニットを接続しないようにRayを照射して接しているか判別している。合体時に使用
    /// </summary>
    /// <returns></returns>
    public bool IsNotIsolatedUnit(){
        if(shapeType == GSetting.ShapeType.Square){
            if(GetUpperGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.RegularTriangle){
            if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.IsoscelesRightTriangle){
            if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.Rectangle){
            if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else if(shapeType == GSetting.ShapeType.ObtusePentagon){
            if(GetDownerGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetRightGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else if(GetLeftGameObject(GSetting.ObjTagName.PlayerUnit.ToString()) != null)return true;
            else return false;
        }else Debug.LogWarning("Cannot Judge IsNotIsolated Bcause ShapeType is Null "); return false;
    }
    /// <summary>
    /// Unitの周りにPlayerUnitがあるかどうかを判別。存在した場合、あとでリンクの更新を行う
    /// PreviewObjectにアタッチする方がよろしいが、ShapeTypeによって処理が異なってしまうのでとりあえずこちらで実装。Intarfaceに直した方がいいかも
    /// </summary>
    /// <returns>隣接するPlayerUnitのUnitBaseのList</returns>
    public List<UnitBase> ReturnAdjacentUnitList(){
        GetAdjacentObjLink(GSetting.ObjTagName.PlayerUnit.ToString());
        List<UnitBase> AdjacentUnitList = new List<UnitBase>();
        if(upperUnit != null)AdjacentUnitList.Add(upperUnit);
        if(downerUnit != null)AdjacentUnitList.Add(downerUnit);
        if(rightUnit != null)AdjacentUnitList.Add(rightUnit);
        if(leftUnit != null)AdjacentUnitList.Add(leftUnit);
        return AdjacentUnitList;
    }
    public int GetDistanseFromCore(){
        return distanseFromCore;
    }
    public void ReRegistData(){
        GetAdjacentObjLink(thisGameObject.tag);
        //UnitData unitData = FM.SearchUnit(this);
        ThisUnitData.ReRegistFourWayLink(upperUnit,downerUnit,rightUnit,leftUnit);
    }
    public void OnTriggerEnter2D(Collider2D collision2D){
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
            //自身がプレイヤーの場合
            case GSetting.ObjTagName.PlayerUnit: 
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    //被弾時の処理
                    case GSetting.ObjTagName.EnemyWeapon1:{
                        WeaponBase HitWeapon = collision2D.GetComponent<WeaponBase>();
                        if(HitPoint >0){
                            SCer.PlaySE(0);
                            //素体ユニットであるかどうかで減算対象を変える
                            if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(HitWeapon.GetAttackPower());}
                            else{
                                DamageCount(HitWeapon.GetTotalDamage());
                                }
                        }
                        break;}
                    case GSetting.ObjTagName.EnemyWeapon2:{
                        WeaponBase HitWeapon = collision2D.GetComponent<WeaponBase>();
                        if(HitPoint >0){
                            SCer.PlaySE(0);
                            //素体ユニットであるかどうかで減算対象を変える
                            if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(HitWeapon.GetAttackPower());}
                            else{
                                DamageCount(HitWeapon.GetTotalDamage());
                                }
                        }
                        break;}
                    case GSetting.ObjTagName.EnemyUnit:{
                        SCer.PlaySE(0);
                        //素体ユニットであるかどうかで減算対象を変える
                        if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(HitPoint);}
                        else{
                        DamageCount(HitPoint);}
                        break;}
                    //プレイヤーのリアクターの効果範囲であれば攻撃倍率を加算する
                    case GSetting.ObjTagName.ReactorEffect:{
                        ReactorBase reactorBase = collision2D.transform.parent.GetComponent<ReactorEffectManager>().GetThisReactor();
                        if(reactorBase == null){break;}
                        if(reactorBase.tag == thisGameObject.tag){
                            AddAttackEfficiency(reactorBase.GetReactorsEfficiencyLevel());}
                        break;}
                    //リアクターが爆発すると敵味方関係なくレベルの分だけダメージを受ける
                    case GSetting.ObjTagName.ReactorExplosion:{
                        ReactorBase reactorBase = collision2D.transform.parent.GetComponent<ReactorEffectManager>().GetThisReactor();
                        if(HitPoint >0){
                            if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(reactorBase.GetReactorsEfficiencyLevel());}
                            else{
                                DamageCount(reactorBase.GetReactorsEfficiencyLevel());
                            }
                        }
                        break;}
                    case GSetting.ObjTagName.CoreExplosion:{
                        CoreBase coreBase = collision2D.transform.parent.GetComponent<CoreEffectManager>().GetThisCore();
                        if(HitPoint > 0){
                            if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(coreBase.GetUnitStatus());}
                            else{
                            DamageCount(coreBase.GetUnitStatus());
                            }
                        }
                        break;}
                    case GSetting.ObjTagName.GrenadeExplosion:{
                        GrenadeEffectManager GEM = collision2D.transform.parent.GetComponent<GrenadeEffectManager>();
                        if(HitPoint > 0){
                            if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(GEM.GetExplosionDamage());}
                            else{
                                DamageCount(GEM.GetExplosionDamage());
                            }
                        }
                        break;}
                    default:break;
                }
                if(HitPoint < 0){HitPoint = 0;}
                break;
            }
            //自身が敵の場合
            case GSetting.ObjTagName.EnemyUnit:
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    case GSetting.ObjTagName.PlayerWeapon1:{
                        WeaponBase HitWeapon = collision2D.GetComponent<WeaponBase>();
                        SCer.PlaySE(0);
                        if(HitPoint >0){DamageCount(HitWeapon.GetTotalDamage());}
                        break;}
                    case GSetting.ObjTagName.PlayerWeapon2:{
                        WeaponBase HitWeapon = collision2D.GetComponent<WeaponBase>();
                        SCer.PlaySE(0);
                        if(HitPoint >0)DamageCount(HitWeapon.GetTotalDamage());
                        break;}
                    case GSetting.ObjTagName.PlayerUnit:{
                        SCer.PlaySE(0);
                        DamageCount(HitPoint);
                        break;}
                    //敵のリアクターの効果範囲であれば攻撃倍率を加算する
                    case GSetting.ObjTagName.ReactorEffect:{
                        ReactorBase reactorBase = collision2D.transform.parent.GetComponent<ReactorEffectManager>().GetThisReactor();
                        if(reactorBase == null){break;}
                        if(reactorBase.tag == thisGameObject.tag){
                            AddAttackEfficiency(reactorBase.GetReactorsEfficiencyLevel());}
                        break;}
                    //リアクターが爆発すると敵味方関係なくレベルの分だけダメージを受ける
                    case GSetting.ObjTagName.ReactorExplosion:{
                        ReactorBase reactorBase = collision2D.transform.parent.GetComponent<ReactorEffectManager>().GetThisReactor();
                        if(HitPoint >0)DamageCount(reactorBase.GetReactorsEfficiencyLevel());
                        break;}
                    //コアが爆発すると敵味方関係なく攻撃力の分だけダメージを受ける
                    case GSetting.ObjTagName.CoreExplosion:{
                        CoreBase coreBase = collision2D.transform.parent.GetComponent<CoreEffectManager>().GetThisCore();
                        if(HitPoint > 0)DamageCount(coreBase.GetUnitStatus());
                        break;
                    }
                    case GSetting.ObjTagName.GrenadeExplosion:{
                        GrenadeEffectManager GEM = collision2D.transform.parent.GetComponent<GrenadeEffectManager>();
                        if(HitPoint > 0){
                            if(isPrime){ReAUDMS.DecreasePrimeUnitsHP(GEM.GetExplosionDamage());}
                            else{
                                DamageCount(GEM.GetExplosionDamage());
                            }
                        }
                        break;}
                    default:break;
                }
                if(HitPoint < 0){HitPoint = 0;}
                break;
            }
            default: break;
        }
    }
    public void OnTriggerExit2D(Collider2D collision2D){
        switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), this.gameObject.tag,true)){
            //自身がプレイヤーの場合
            case GSetting.ObjTagName.PlayerUnit: 
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    //リアクターが分離または爆発した際に効果倍率を減算する
                    case GSetting.ObjTagName.ReactorEffect:{
                        ReactorBase reactorBase = collision2D.transform.parent.GetComponent<ReactorEffectManager>().GetThisReactor();
                        if(reactorBase == null){break;}
                        //引数はマイナスにすること
                        if(reactorBase.tag == thisGameObject.tag){
                            if(HitPoint <= 0){break;}//HPが0以下ならばこの処理を行ったところで意味がないし、コルーチンの無駄な呼び出しになるから
                            AddAttackEfficiency( - reactorBase.GetReactorsEfficiencyLevel());}
                        break;}
                    default:break;
                }
                break;
            }
            case GSetting.ObjTagName.EnemyUnit:
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    //リアクターが分離または爆発した際に効果倍率を減算する
                    case GSetting.ObjTagName.ReactorEffect:{
                        ReactorBase reactorBase = collision2D.transform.parent.GetComponent<ReactorEffectManager>().GetThisReactor();
                        if(reactorBase == null){break;}
                        //引数はマイナスにすること
                        if(reactorBase.tag == thisGameObject.tag){
                            if(HitPoint <= 0){break;}//HPが0以下ならばこの処理を行ったところで意味がないし、コルーチンの無駄な呼び出しになるから
                            AddAttackEfficiency( - reactorBase.GetReactorsEfficiencyLevel());}
                        break;}
                    default:break;
                }
                break;
            }
            case GSetting.ObjTagName.DestroyedUnit:
            {
                switch((GSetting.ObjTagName)Enum.Parse(typeof(GSetting.ObjTagName), collision2D!.transform.tag,true)){
                    default:break;
                }
                break;
            }
            default:break;
        }
    }
    /// <summary>
    /// 被弾するとダメージ値を表示する+ダメージ処理を行う
    /// </summary>
    /// <param name="damagePoint"></param>
    private void DamageCount(int damagePoint){
        float fadeTime = 0.5f;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(this.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRectTransform, 
            screenPosition, 
            Camera.main, 
            out Vector2 localPosition
        );
        GameObject DamageObj = Instantiate(DamageCountText,uiRectTransform.transform);
        DamageObj.GetComponent<RectTransform>().anchoredPosition = screenPosition + localPosition + new Vector2(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
        TextMeshProUGUI Text = DamageObj.GetComponent<TextMeshProUGUI>();
        Text.text = damagePoint.ToString();
        Text.DOFade(0f,fadeTime);
        TakeDamage(damagePoint);
        SCer.PlaySE(0);
    }
    public virtual void TakeDamage(int damagePoint){
        HitPoint -= damagePoint;
    }
    /// <summary>
    ///デバッグ用の被弾処理。それ以外では使わないこと
    /// </summary>
    public void DebugDamaged(){
        //素体ユニットであるかどうかで減算対象を変える
        if(isPrime){ReAUDMS?.DecreasePrimeUnitsHP(1);}
        else{DamageCount(1);}
    }
}
