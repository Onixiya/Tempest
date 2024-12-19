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
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Audio;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
[assembly:MelonGame("Ninja Kiwi","BloonsTD6")]
[assembly:MelonInfo(typeof(Tempest.SC2ModMain),Tempest.ModHelperData.Name,Tempest.ModHelperData.Version,"Silentstorm")]
namespace Tempest{
    public class SC2ModMain:MelonMod{
    }
    public class Tempest:SC2Tower{
        public override string Name=>"Tempest";
        public override Faction TowerFaction=>Faction.Protoss;
        public override UpgradeModel[]GenerateUpgradeModels(){
            return new UpgradeModel[]{
                new("Disruption Blast",980,0,new(){guidRef="Ui["+Name+"-DisruptionBlastIcon]"},0,1,0,"","Disruption Blast"),
                new("Tectonic Destablizers",1875,0,new(){guidRef="Ui["+Name+"-TectonicDestablizersIcon]"},0,2,0,"","Tectonic Destablizers"),
                new("Disintegration",7840,0,new(){guidRef="Ui["+Name+"-DisintegrationIcon]"},0,3,0,"","Disintegration"),
                new("Antimatter Infusion",18575,0,new(){guidRef="Ui["+Name+"-AntimatterInfusionIcon]"},0,4,0,"","Antimatter Infusion")
            };
        }
        public override int MaxTier=>4;
        public override ShopTowerDetailsModel ShopDetails(){
            ShopTowerDetailsModel details=gameModel.towerSet[0].Clone<ShopTowerDetailsModel>();
            details.towerId=Name;
            details.name=Name;
            details.towerIndex=12;
            details.pathOneMax=4;
            details.pathTwoMax=0;
            details.pathThreeMax=0;
			LocManager.textTable.Add("Tempest Description","Protoss aerial seige craft, very high damage and range with slow fire rate");
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
            tempest.display=new(){guidRef=Name+"-Prefab"};
            tempest.icon=new(){guidRef="Ui["+Name+"-Icon]"};
            tempest.instaIcon=new(){guidRef="Ui["+Name+"-Icon]"};
            tempest.portrait=new(){guidRef="Ui["+Name+"-Portrait]"};
            List<Model>tempestBehav=tempest.behaviors.ToList();
            tempestBehav.Add(SelectedSoundModel);
            DisplayModel display=tempestBehav.GetModel<DisplayModel>();
            display.positionOffset=new(0,0,190);
            display.display=tempest.display;
            AttackModel attackModel=tempestBehav.GetModel<AttackModel>();
            attackModel.range=tempest.range;
            attackModel.behaviors.GetModel<RotateToTargetModel>().onlyRotateDuringThrow=false;
            WeaponModel weapon=attackModel.weapons[0];
            weapon.rate=2;
            weapon.ejectZ=10;
            ProjectileModel proj=weapon.projectile;
            proj.pierce=1;
            proj.display=new(){guidRef=Name+"-BallPrefab"};
            proj.collisionPasses[0]=0;
            Il2CppReferenceArray<Model>projBehav=proj.behaviors;
            DamageModel damage=projBehav.GetModel<DamageModel>();
            damage.damage=40;
            damage.immuneBloonProperties=(BloonProperties)8;
            TravelStraitModel travelModel=projBehav.GetModel<TravelStraitModel>();
            travelModel.speed*=2.5f;
            travelModel.lifespan=5;
            tempest.behaviors=tempestBehav.ToArray();
            SetSounds(tempest,Name+"-",true,true,true,false);
            return tempest;
        }
        public TowerModel DisruptionBlast(){
            TowerModel tempest=Base().Clone().Cast<TowerModel>();
            tempest.name=Name+"-100";
            tempest.tier=1;
            tempest.tiers=new int[]{1,0,0};
            tempest.appliedUpgrades=new(new[]{"Disruption Blast"});
            tempest.upgrades=new UpgradePathModel[]{new("Tectonic Destablizers",Name+"-200")};
            ProjectileModel proj=tempest.behaviors.GetModel<AttackModel>().weapons[0].projectile;
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
            ProjectileModel proj=tempest.behaviors.GetModel<AttackModel>().weapons[0].projectile;
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
            AbilityModel disintegration=BlankAbilityModel;
            AttackModel disintegrationAttack=tempest.behaviors.GetModel<AttackModel>().Clone<AttackModel>();
            disintegrationAttack.weapons[0].projectile=gameModel.GetTowerFromId("DartlingGunner-200").behaviors.
                GetModel<AttackModel>().weapons[0].projectile.Clone<ProjectileModel>();
            AddBehaviorToBloonModel AddDOT=disintegrationAttack.weapons[0].projectile.behaviors.GetModel<AddBehaviorToBloonModel>();
            DamageOverTimeModel DOT=disintegrationAttack.weapons[0].projectile.behaviors.GetModel<AddBehaviorToBloonModel>().behaviors.
                First(a=>a.GetIl2CppType()==Il2CppType.Of<DamageOverTimeModel>()).Cast<DamageOverTimeModel>();
            AddDOT.overlayType="";
            AddDOT.lifespan=10;
            AddDOT.stackCount=1;
            DOT.damage=25;
            DOT.Interval=0.5f;
            DOT.immuneBloonProperties=(BloonProperties)8;
            DOT.displayPath.guidRef="";
            DOT.displayLifetime=0;
            disintegrationAttack.weapons[0].projectile.display=new(){guidRef=Name+"-DisintegrationPrefab"};
            List<Model>disintBehaviors=disintegration.behaviors.ToList();
            disintBehaviors.Add(new ActivateAttackModel("ActivateAttackModel",2,true,new(new[]{disintegrationAttack}),true,true,false,false,true,false));
            disintegration.behaviors=disintBehaviors.ToArray();
            disintegration.name="Disintegration";
            disintegration.displayName=disintegration.name;
            disintegration.cooldown=80;
            disintegration.description="Disintegration Description";
            disintegration.icon=new(){guidRef="Ui["+Name+"-DisintegrationIcon]"};
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
            tempest.portrait=new(){guidRef="Ui["+Name+"-UpgradedPortrait]"};
            tempest.display=new(Name+"-UpgradedPrefab");
            Il2CppReferenceArray<Model>tempestBehav=tempest.behaviors;
            tempestBehav.GetModel<DisplayModel>().display=tempest.display;
            ProjectileModel tempestProj=tempestBehav.GetModel<AttackModel>().weapons[0].projectile;
            DamageModel tempestDamage=tempestProj.behaviors.GetModel<DamageModel>();
            tempestDamage.damage*=2;
            tempestDamage.immuneBloonProperties=0;
            tempestProj.behaviors.GetModel<SlowMaimMoabModel>().badDuration=3;
            ProjectileModel disintProj=tempestBehav.GetModel<AbilityModel>().behaviors.GetModel<ActivateAttackModel>().attacks[0].weapons[0].projectile;
            disintProj.behaviors.GetModel<DamageModel>().immuneBloonProperties=0;
            disintProj.behaviors.GetModel<AddBehaviorToBloonModel>().
                behaviors.First(a=>a.GetIl2CppType()==Il2CppType.Of<DamageOverTimeModel>()).Cast<DamageOverTimeModel>().immuneBloonProperties=0;
            return tempest;
        }
        public override void Attack(Weapon weapon){
            PlayAnimation(weapon.attack.tower.Node.graphic.GetComponent<Animator>(),Name+"-Attack");
        }
        public override bool Ability(string ability,Tower tower){
            PlayAnimation(tower.Node.graphic.GetComponent<Animator>(),Name+"-Attack");
			return true;
        }
    }
}