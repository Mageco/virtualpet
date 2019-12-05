
// Item
public enum ItemSkillType {NONE, LEARN, USE};
public enum ItemEventType{None,Eat,Drink,Rest}
public enum ItemType{All=-1,Diamond=0,Coin=1,Food=2,Drink=3,Toy=4,Clean=5,MedicineBox=6,Room=7,Bed=8,Toilet=9,Bath=10,Picture=11,Table=12,Clock=13};
public enum PriceType{Money,Coin,Diamond}
public enum ItemState {OnShop,Have,Equiped,Locked}
public enum ItemDragState{None,Drag,Drop,Fall,Hit,Hited,Active,Highlight};
public enum InteractType {None,Drag,Drop,Touch,Jump,DoubleClick};
public enum QuestRequirementType{Action,Interact,Skill,Variable};
public enum SkillType {NONE,Toilet,Sleep,Call,Bath,Table};
public enum AchivementType {Do_Action,Buy_Item,Use_Item,Tap_Animal,Dissmiss_Animal,LevelUp,Minigame_Level,Eat,Drink,Play_MiniGame};
public enum Direction {R =0, D = 1, L = 2, U = 3, RU = 4,RD = 5,LU = 6,LD = 7};
public enum CharAge{Small,Middle,Big};
public enum EnviromentType { Room, Table, Bath,Bed, Toilet};
public enum GameType{House,Garden,Minigame1};
public enum GameState{Prepare,Ready,Run,End};
public enum ActionType { None, Mouse, Rest, Sleep, Eat, Drink, Patrol, Discover, Pee, Shit, Itchi, Sick, Sad, Fear, Happy, Tired, Call, Hold, OnTable, Bath, Listening,Fall,SkillUp,LevelUp,OnBed,OnToilet,Injured}

public enum WeatherType{None,Sunny,Rain,Snow,Storm}
public enum AnimalState {None,Idle,Seek,Eat,Run,Flee,Hit,Hit_Grab,Hold,Cached,InActive }
public enum SickType{Sick,Injured};
public enum AnimalType{Mouse,Fox,Eagle,Snake};
public enum RewardState{None,Ready,Received};
public enum IconStatus{None,Hungry_1,Hungry_2,Sick_1,Sick_2,Dirty_1,Dirty_2,Toilet_1,Toilet_2};


