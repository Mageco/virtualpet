
// Item
public enum ValueSetter {PERCENT, VALUE};
public enum ItemSkillType {NONE, LEARN, USE};
public enum ItemVariableType {NONE, SET, REMOVE};
public enum ItemDropType {NONE, ITEM, EQUIPMENT, SKILL};
public enum ColumnFill {VERTICAL, HORIZONTAL};
public enum ItemType{None,Diamond,Coin,Food,Drink,Toy,Dog,House,Room,Bed,Toilet,Bath,Picture,Cleaner,Table,Clock};
public enum PriceType{Money,Coin,Diamond}
public enum ItemState {Buy,Use,Used}

// status values
public enum StatusValueType {NORMAL, CONSUMABLE, EXPERIENCE};

// status effect
public enum StatusEffectEnd {NONE, TIME};
public enum StatusConditionExecution {CAST,TIME};
public enum StatusNeeded {STATUS_VALUE, SKILL};

// skill
public enum SkillEffect {NONE, ADD, REMOVE};
public enum SkillType {NONE,Toilet,Pee,Eat,Drink,Sleep,Call,Toy_Car,Toy_Circle,Toy_Ball};
public enum Direction {R =0, D = 1, L = 2, U = 3, RU = 4,RD = 5,LU = 6,LD = 7};

//Movement
public enum MovementType{WALK = 0,RUN = 1,CRAW = 2,JUMP = 3};

// formula
public enum SimpleOperator {ADD, SUB, SET};
public enum FormulaChooser {VALUE, STATUS, RANDOM};
public enum StatusOrigin {USER, TARGET};
public enum FormulaOperator {DONE, ADD, SUB, MULTIPLY, DIVIDE, MODULO, POWER_OF, LOG};
public enum Rounding {NONE, CEIL, FLOOR, ROUND};
public enum ValueCheck {EQUALS, LESS, GREATER};

// event
public enum EventStartType {INTERACT, AUTOSTART, TRIGGER_ENTER};
