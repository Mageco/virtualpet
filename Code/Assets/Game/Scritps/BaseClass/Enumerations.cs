
// Item
public enum ItemSkillType {NONE, LEARN, USE};
public enum ItemEventType{None,Eat,Drink,Rest}
public enum ItemType{All=-1,Diamond=0,Coin=1,Food=2,Drink=3,Toy=4,Clean=5,MedicineBox=6,Room=7,Bed=8,Toilet=9,Bath=10,Picture=11,Table=12,Clock=13,Animal=14,Fruit = 15,MagicBox = 16,Chest = 17,Deco=18,Board=19,Gate=20};
public enum PriceType{Money,Coin,Diamond,Happy}
public enum ItemState {OnShop,Have,Equiped,Locked}
public enum ItemDragState{None,Drag,Drop,Fall,Hit,Hited,Active,Highlight};
public enum InteractType {None,Drag,Drop,Touch,Jump,DoubleClick,Fly,Toy,Love};
public enum QuestRequirementType{Action,Interact,Skill,Variable};
public enum SkillType {NONE,Toilet,Sleep,Call,Bath,Table};
public enum AchivementType {Do_Action,Buy_Item,Use_Item,Tap_Animal,Dissmiss_Animal,LevelUp,Minigame_Level,Eat,Drink,Play_MiniGame,Sick,Injured,Clean,CollectHeart,CollectFruit};
public enum Direction {R =0, D = 1, L = 2, U = 3, RU = 4,RD = 5,LU = 6,LD = 7};
public enum CharAge{Small,Middle,Big};
public enum EnviromentType { Room, Table, Bath,Bed, Toilet,Door,ToHouse};
public enum GameState{Prepare,Ready,Run,End};
public enum ActionType { None, Mouse, Sleep, Eat, Drink, Patrol, Discover, Pee, Shit, Itchi, Sick, Fear, Happy, Tired, Hold, OnTable, OnBath,Fall,OnBed,OnToilet,Injured,Supprised,Stop,Toy,JumpOut,OnCall,OnControl,OnGarden,OnGift,OnCity};
public enum CharType{Dog,Cat,Rabbit,Turtle,Mouse,Parrot,Hamster,Chihuhu,Unicorn,Boar,ButterFly,Shamoyed,PersianCat};
public enum WeatherType{None,Sunny,Rain,Snow,Storm}
public enum AnimalState {None,Idle,Seek,Eat,Run,Flee,Hit,Hit_Grab,Hold,Cached,InActive }
public enum SickType{Sick,Injured};
public enum AnimalType{Mouse,Fox,Eagle,Snake,Chicken,Bee};
public enum RewardState{None,Ready,Received};
public enum IconStatus{None,Hungry_1,Hungry_2,Sick_1,Sick_2,Dirty_1,Dirty_2,Toilet_1,Toilet_2,Thirsty_1,Thirsty_2,Sleepy_1,Sleepy_2,Tired_1,Tired_2,Injured_1,Injured_2};
public enum ToyType{Jump,Ball,Car,Doll,SpaceShip,Wheel,Robot,Slider,Spring,Dance,Swing,Circle,Fun,Seesaw,Carrier,Sprinkler,Flying};
public enum ItemSaveDataType{None,Pee,Shit,Rubbish,Food,Drink,Happy,Fruit,Chest,Toy,Equipment,Decor};
public enum EquipmentState { Idle,Hold, Drag, Busy, Active }
public enum MovementType {TwoDirection,FourDirection,None};
public enum MapType {House = 0,Forest=1,Lake=2,City=3,Mountain=4,Village=5,Cave=6,SunkenShip=7,Habor=8};
public enum RewardType {None, Minigame, Chest, Sick, Injured, Map,Welcome,Service,ForestDiamond,SpinWheel};
public enum ServiceType {Instructor,Doctor,Chef,HouseKeeper,PetSitter,Exp};
public enum AdDistribute {None,Unity,Admob};
public enum RareType { Common, Rare,Epic,Legend};
public enum AreaType {Room,Garden,Lake,All,Camera,Fly};
public enum ItemTag {None=0,Hot=1,Sale=2,New=3};
public enum DailyQuestState {None,Started,Ready,Collected};