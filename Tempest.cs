using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppNinjaKiwi.Common;
using Il2CppAssets.Scripts.Models.Map;
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
[assembly:MelonGame("Ninja Kiwi","BloonsTD6")]
[assembly:MelonInfo(typeof(Tempest.ModMain),Tempest.ModHelperData.Name,Tempest.ModHelperData.Version,"Silentstorm")]
namespace Tempest{
    public class ModMain:MelonMod{
        public static string LoaderPath=MelonEnvironment.ModsDirectory+"/SC2ExpansionLoader.dll";
        public override async void OnEarlyInitializeMelon(){
            if(!File.Exists(LoaderPath)){
                var httpClient=new HttpClient();
                using(var stream=await httpClient.GetStreamAsync("https://github.com/Onixiya/SC2Expansion/releases/download/latest/SC2ExpansionLoader.dll")){
                    using(var fileStream=new FileStream(LoaderPath, FileMode.CreateNew)){
                        await stream.CopyToAsync(fileStream);
                    }
                }
                MelonAssembly.LoadMelonAssembly(LoaderPath);
            }
        }
    }
    public class Tempest:SC2Tower{
        public override string Name=>"Tempest";
        public override UpgradeModel[]Upgrades(){
            return new UpgradeModel[]{
                new("Disruption Blast",980,0,new(){guidRef="Ui[Tempest-DisruptionBlastIcon]"},0,1,0,"","Disruption Blast"),
                new("Tectonic Destablizers",1875,0,new(){guidRef="Ui[Tempest-TectonicDestablizersIcon]"},0,2,0,"","Tectonic Destablizers"),
                new("Disintegration",7840,0,new(){guidRef="Ui[Tempest-DisintegrationIcon]"},0,3,0,"","Disintegration"),
                new("Antimatter Infusion",18575,0,new(){guidRef="Ui[Tempest-AntimatterInfusionIcon]"},0,4,0,"","Antimatter Infusion")
            };
        }
        public override Dictionary<string,string>SoundNames=>new(){{"Tempest","Tempest-"},{"Upgraded","Tempest-"}};
        public override ShopTowerDetailsModel ShopDetails(){
            ShopTowerDetailsModel details=Game.instance.model.towerSet[0].Clone().Cast<ShopTowerDetailsModel>();
            details.towerId=Name;
            details.name=Name;
            details.towerIndex=12;
            details.pathOneMax=4;
            details.pathTwoMax=0;
            details.pathThreeMax=0;
            details.popsRequired=0;
            LocalizationManager.Instance.textTable.Add("Disruption Blast Description","Attacks slow targets for a short time");
            LocalizationManager.Instance.textTable.Add("Tectonic Destablizers Description","Deals triple damage against Moab class Bloons");
            LocalizationManager.Instance.textTable.Add("Disintegration Description","Ability: Fires a massive amount of energy at a target dealing huge damage to it over a short period");
            LocalizationManager.Instance.textTable.Add("Antimatter Infusion Description","Doubles damage dealt, damage all bloon types and slows down B.A.D's");
            return details;
        }
        public override TowerModel[]TowerModels(){
            return new TowerModel[]{
                Base(),
                DisruptionBlast(),
                TectonicDestablizers(),
                Disintegration(),
                AntimatterInfusion()
            };
        }
        public TowerModel Base(){
            TowerModel tempest=Game.instance.model.GetTowerFromId("DartMonkey").Clone().Cast<TowerModel>();
            tempest.name=Name;
            tempest.baseId=tempest.name;
            tempest.towerSet=TowerSet.Magic;
            tempest.cost=1750;
            tempest.tier=0;
            tempest.tiers=new[]{0,0,0};
            tempest.upgrades=new UpgradePathModel[]{new("Disruption Blast",Name+"-100")};
            tempest.range=90;
            tempest.display=new(){guidRef="Tempest-Tempest-Prefab"};
            tempest.icon=new(){guidRef="Ui[Tempest-Icon]"};
            tempest.instaIcon=new(){guidRef="Ui[Tempest-Icon]"};
            tempest.portrait=new(){guidRef="Ui[Tempest-Portrait]"};
            DisplayModel display=tempest.behaviors.First(a=>a.GetIl2CppType().Name=="DisplayModel").Cast<DisplayModel>();
            display.positionOffset=new(0,0,190);
            display.display=new(){guidRef="Tempest-Tempest-Prefab"};
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
            disintegrationAttack.weapons[0].projectile=Game.instance.model.GetTowerFromId("DartlingGunner-200").behaviors.
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
            tempest.display=new(){guidRef="Tempest-Upgraded-Prefab"};
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
        public override void Create(){
            PlaySound("Tempest-Birth");
        }
        public override void Upgrade(int tier,Tower tower){
            tower.Node.graphic.gameObject.GetComponent<SC2Sound>().PlayUpgradeSound();
        }
        public override void Select(Tower tower){
            tower.Node.graphic.gameObject.GetComponent<SC2Sound>().PlaySelectSound();
        }
        public override void Attack(Weapon weapon){
            PlayAnimation(weapon.attack.tower.Node.graphic,"Tempest-Attack");
        }
        public override void Ability(string ability,Tower tower){
            PlaySound("Tempest-Disintegration");
            PlayAnimation(tower.Node.graphic,"Tempest-Attack");
        }
    }
}