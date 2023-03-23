using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppInterop.Runtime;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models;
using MelonLoader;
using SC2ExpansionLoader;
using static SC2ExpansionLoader.ModMain;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using Il2Cpp;
using MelonLoader.Utils;
using UnityEngine;
[assembly:MelonGame("Ninja Kiwi","BloonsTD6")]
[assembly:MelonInfo(typeof(Tempest.ModMain),Tempest.ModHelperData.Name,Tempest.ModHelperData.Version,"Silentstorm")]
[assembly:MelonOptionalDependencies("SC2ExpansionLoader")]
namespace Tempest{
    public class ModMain:MelonMod{
        public static string LoaderPath=MelonEnvironment.ModsDirectory+"/SC2ExpansionLoader.dll";
        public override void OnEarlyInitializeMelon(){
            if(!File.Exists(LoaderPath)){
                var httpClient=new HttpClient();
                var stream=httpClient.GetStreamAsync("https://github.com/Onixiya/SC2Expansion/releases/latest/download/SC2ExpansionLoader.dll");
                var fileStream=new FileStream(LoaderPath,FileMode.CreateNew);
                stream.Result.CopyToAsync(fileStream);
                Log("Restarting game so MelonLoader correctly loads all mods");
                Application.Quit();
            }
        }
    }
    public class Tempest:SC2Tower{
        public override string Name=>"Tempest";
        public override string Description=>"Protoss aerial seige craft, very high damage and range with slow fire rate";
        public override Faction TowerFaction=>Faction.Protoss;
        public override UpgradeModel[]GenerateUpgradeModels(){
            return new UpgradeModel[]{
                new("Disruption Blast",980,0,new(){guidRef="Ui[Tempest-DisruptionBlastIcon]"},0,1,0,"","Disruption Blast"),
                new("Tectonic Destablizers",1875,0,new(){guidRef="Ui[Tempest-TectonicDestablizersIcon]"},0,2,0,"","Tectonic Destablizers"),
                new("Disintegration",7840,0,new(){guidRef="Ui[Tempest-DisintegrationIcon]"},0,3,0,"","Disintegration"),
                new("Antimatter Infusion",18575,0,new(){guidRef="Ui[Tempest-AntimatterInfusionIcon]"},0,4,0,"","Antimatter Infusion")
            };
        }
		public override Dictionary<string,Il2CppSystem.Type>Components=>new(){{"Tempest-Prefab",Il2CppType.Of<TempestCom>()}};
		[RegisterTypeInIl2Cpp]
        public class TempestCom:MonoBehaviour{
            public TempestCom(IntPtr ptr):base(ptr){}
			public GameObject activeObj=null;
			public GameObject tempest=null;
			public GameObject upgradeTempest=null;
			int selectSound=0;
			int upgradeSound=0;
			void Start(){
				tempest=transform.GetChild(0).gameObject;
				upgradeTempest=transform.GetChild(1).gameObject;
				upgradeTempest.SetActive(false);
				tempest.transform.localPosition=new(0,0,0);
				upgradeTempest.transform.localPosition=new(0,0,0);
				activeObj=tempest;
			}
			public void PlaySelectSound(){
				if(selectSound>5){
					selectSound=0;
				}
				selectSound+=1;
				PlaySound("Tempest-Select"+selectSound);
			}
			public void PlayUpgradeSound(){
				upgradeSound+=1;
				selectSound=0;
				PlaySound("Tempest-Upgrade"+upgradeSound);
			}
        }
        public override int MaxTier=>4;
        public override ShopTowerDetailsModel ShopDetails(){
            ShopTowerDetailsModel details=Game.instance.model.towerSet[0].Clone().Cast<ShopTowerDetailsModel>();
            details.towerId=Name;
            details.name=Name;
            details.towerIndex=12;
            details.pathOneMax=4;
            details.pathTwoMax=0;
            details.pathThreeMax=0;
            details.popsRequired=0;
            LocManager.textTable.Add("Disruption Blast Description","Attacks slow targets for a short time");
            LocManager.textTable.Add("Tectonic Destablizers Description","Deals triple damage against Moab class Bloons");
            LocManager.textTable.Add("Disintegration Description","Ability: Fires a massive amount of energy at a target dealing huge damage to it over a short period");
            LocManager.textTable.Add("Antimatter Infusion Description","Doubles damage dealt, damage all bloon types and slows down B.A.D's");
            return details;
        }
        public override TowerModel[]GenerateTowerModels(){
            return new TowerModel[]{
                Base(),
                DisruptionBlast(),
                TectonicDestablizers(),
                Disintegration(),
                AntimatterInfusion()
            };
        }
        public TowerModel Base(){
            TowerModel tempest=gameModel.GetTowerFromId("DartMonkey").Clone().Cast<TowerModel>();
            tempest.name=Name;
            tempest.baseId=tempest.name;
            tempest.towerSet=TowerSet.Magic;
            tempest.cost=1750;
            tempest.tier=0;
            tempest.tiers=new[]{0,0,0};
            tempest.upgrades=new UpgradePathModel[]{new("Disruption Blast",Name+"-100")};
            tempest.range=90;
            tempest.display=new(){guidRef="Tempest-Prefab"};
            tempest.icon=new(){guidRef="Ui[Tempest-Icon]"};
            tempest.instaIcon=new(){guidRef="Ui[Tempest-Icon]"};
            tempest.portrait=new(){guidRef="Ui[Tempest-Portrait]"};
            DisplayModel display=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="DisplayModel").Cast<DisplayModel>();
            display.positionOffset=new(0,0,190);
            display.display=new(){guidRef="Tempest-Prefab"};
            AttackModel attackModel=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="AttackModel").Cast<AttackModel>();
            attackModel.range=tempest.range;
            attackModel.behaviors.First(a=>a.GetIl2CppType().Name=="RotateToTargetModel").Cast<RotateToTargetModel>().onlyRotateDuringThrow=false;
            WeaponModel weapon=attackModel.weapons[0];
            weapon.rate=2;
            weapon.ejectZ=10;
            ProjectileModel proj=weapon.projectile;
            proj.pierce=1;
            proj.display=new(){guidRef="Tempest-BallPrefab"};
            proj.collisionPasses[0]=0;
            DamageModel damage=proj.behaviors.First(a=>a.GetIl2CppType().Name=="DamageModel").Cast<DamageModel>();
            damage.damage=40;
            damage.immuneBloonProperties=(BloonProperties)8;
            TravelStraitModel travelModel=proj.behaviors.First(a=>a.GetIl2CppType().Name=="TravelStraitModel").Cast<TravelStraitModel>();
            travelModel.speed*=2.5f;
            travelModel.lifespan=5;
            return tempest;
        }
        public TowerModel DisruptionBlast(){
            TowerModel tempest=Base().Clone().Cast<TowerModel>();
            tempest.name=Name+"-100";
            tempest.tier=1;
            tempest.tiers=new int[]{1,0,0};
            tempest.appliedUpgrades=new(new[]{"Disruption Blast"});
            tempest.upgrades=new UpgradePathModel[]{new("Tectonic Destablizers",Name+"-200")};
            ProjectileModel proj=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="AttackModel").Cast<AttackModel>().weapons[0].projectile;
            List<Model>projBehaviors=proj.behaviors.ToList();
            projBehaviors.Add(new SlowMaimMoabModel("",0,0,0,0,0,0,0,""){name="SlowModel",moabDuration=3,bfbDuration=3,zomgDuration=3,
                ddtDuration=3,badDuration=0,multiplier=0.5f,bloonPerHitDamageAddition=0,overlayType=""});
            proj.behaviors=projBehaviors.ToArray();
            return tempest;
        }
        public TowerModel TectonicDestablizers(){
            TowerModel tempest=DisruptionBlast().Clone().Cast<TowerModel>();
            tempest.name=Name+"-200";
            tempest.tier=2;
            tempest.tiers=new int[]{2,0,0};
            tempest.appliedUpgrades=new(new[]{"Disruption Blast","Tectonic Destablizers"});
            tempest.upgrades=new UpgradePathModel[]{new("Disintegration",Name+"-300")};
            ProjectileModel proj=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="AttackModel").Cast<AttackModel>().weapons[0].projectile;
            List<Model>projBehaviors=proj.behaviors.ToList();
            projBehaviors.Add(new DamageModifierForTagModel(null,null,0,0,false,false){name="DamageModifierForTagModel",tag="Moabs",
                damageMultiplier=3,damageAddative=0,mustIncludeAllTags=false,applyOverMaxDamage=true});
            proj.behaviors=projBehaviors.ToArray();
            return tempest;
        }
        public TowerModel Disintegration(){
            TowerModel tempest=TectonicDestablizers().Clone().Cast<TowerModel>();
            tempest.name=Name+"-300";
            tempest.tier=3;
            tempest.tiers=new int[]{3,0,0};
            tempest.appliedUpgrades=new(new[]{"Disruption Blast","Tectonic Destablizers","Disintegration"});
            tempest.upgrades=new UpgradePathModel[]{new("Antimatter Infusion",Name+"-400")};
            AbilityModel disintegration=BlankAbilityModel.Clone().Cast<AbilityModel>();
            AttackModel disintegrationAttack=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="AttackModel").Clone().Cast<AttackModel>();
            disintegrationAttack.weapons[0].projectile=gameModel.GetTowerFromId("DartlingGunner-200").behaviors.
                First(a=>a.GetIl2CppType().Name=="AttackModel").Cast<AttackModel>().weapons[0].projectile.Clone().Cast<ProjectileModel>();
            AddBehaviorToBloonModel AddDOT=disintegrationAttack.weapons[0].projectile.behaviors.First(a=>a.GetIl2CppType().Name=="AddBehaviorToBloonModel").
                Cast<AddBehaviorToBloonModel>();
            DamageOverTimeModel DOT=disintegrationAttack.weapons[0].projectile.behaviors.First(a=>a.GetIl2CppType().Name=="AddBehaviorToBloonModel").
                Cast<AddBehaviorToBloonModel>().behaviors.First(a=>a.GetIl2CppType().Name=="DamageOverTimeModel").Cast<DamageOverTimeModel>();
            AddDOT.overlayType="";
            AddDOT.lifespan=10;
            AddDOT.stackCount=1;
            DOT.damage=25;
            DOT.Interval=0.5f;
            DOT.immuneBloonProperties=(BloonProperties)8;
            DOT.displayPath.guidRef="";
            DOT.displayLifetime=0;
            disintegrationAttack.weapons[0].projectile.display=new(){guidRef="Tempest-DisintegrationBallPrefab"};
            List<Model>disintBehaviors=disintegration.behaviors.ToList();
            disintBehaviors.Add(new ActivateAttackModel(null,69,false,null,false,false,false,false,false){name="ActivateAttackModel",
                lifespan=2,processOnActivate=true,attacks=new(new[]{disintegrationAttack}),cancelIfNoTargets=true,turnOffExisting=true,
                endOnDefeatScreen=false,isOneShot=true});
            disintegration.behaviors=disintBehaviors.ToArray();
            disintegration.name="Disintegration";
            disintegration.displayName=disintegration.name;
            disintegration.cooldown=80;
            disintegration.description="Disintegration Description";
            disintegration.icon=new(){guidRef="Ui[Tempest-DisintegrationIcon]"};
            List<Model>tempestBehaviors=tempest.behaviors.ToList();
            tempestBehaviors.Add(disintegration);
            tempest.behaviors=tempestBehaviors.ToArray();
            return tempest;
        }
        public TowerModel AntimatterInfusion(){
            TowerModel tempest=Disintegration().Clone().Cast<TowerModel>();
            tempest.name=Name+"-400";
            tempest.tier=4;
            tempest.tiers=new int[]{4,0,0};
            tempest.appliedUpgrades=new(new[]{"Disruption Blast","Tectonic Destablizers","Disintegration","Antimatter Infusion"});
            tempest.upgrades=new(0);
            tempest.portrait=new(){guidRef="Ui[Tempest-UpgradedPortrait]"};
            ProjectileModel proj=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="AttackModel").Cast<AttackModel>().weapons[0].projectile;
            DamageModel damage=proj.behaviors.First(a=>a.GetIl2CppType().Name=="DamageModel").Cast<DamageModel>();
            damage.damage*=2;
            damage.immuneBloonProperties=0;
            proj.behaviors.First(a=>a.GetIl2CppType().Name=="SlowMaimMoabModel").Cast<SlowMaimMoabModel>().badDuration=3;
            ProjectileModel disintProj=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="AbilityModel").Cast<AbilityModel>().behaviors.
                First(a=>a.GetIl2CppType().Name=="ActivateAttackModel").Cast<ActivateAttackModel>().attacks[0].weapons[0].projectile;
            disintProj.behaviors.First(a=>a.GetIl2CppType().Name=="DamageModel").Cast<DamageModel>().immuneBloonProperties=0;
            disintProj.behaviors.First(a=>a.GetIl2CppType().Name=="AddBehaviorToBloonModel").Cast<AddBehaviorToBloonModel>().
                behaviors.First(a=>a.GetIl2CppType().Name=="DamageOverTimeModel").Cast<DamageOverTimeModel>().immuneBloonProperties=0;
            return tempest;
        }
        public override void Create(Tower tower){
            PlaySound("Tempest-Birth");
        }
        public override void Upgrade(int tier,Tower tower){
			TempestCom com=tower.Node.graphic.gameObject.GetComponent<TempestCom>();
			if(tier==4){
				com.activeObj.SetActive(false);
				com.activeObj=com.upgradeTempest;
				com.activeObj.SetActive(true);
			}
            com.PlayUpgradeSound();
        }
        public override void Select(Tower tower){
            tower.Node.graphic.gameObject.GetComponent<TempestCom>().PlaySelectSound();
        }
        public override void Attack(Weapon weapon){
            PlayAnimation(weapon.attack.tower.Node.graphic.GetComponent<TempestCom>().activeObj.GetComponent<Animator>(),"Tempest-Attack");
        }
        public override bool Ability(string ability,Tower tower){
            PlaySound("Tempest-Disintegration");
            PlayAnimation(tower.Node.graphic.GetComponent<TempestCom>().activeObj.GetComponent<Animator>(),"Tempest-Attack");
			return true;
        }
    }
}