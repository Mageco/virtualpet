
// Item
public enum ValueSetter {PERCENT, VALUE};
public enum ItemSkillType {NONE, LEARN, USE};
public enum ItemVariableType {NONE, SET, REMOVE};
public enum ItemDropType {NONE, ITEM, EQUIPMENT, SKILL};
public enum ColumnFill {VERTICAL, HORIZONTAL};
public enum ItemType{Diamond=0,Food=1,Toys=2,Dogs=3,Set=4,Room=5,Bed=6,Bath=7,Other=8}


// status values
public enum StatusValueType {NORMAL, CONSUMABLE, EXPERIENCE};

// status effect
public enum StatusEffectEnd {NONE, TIME};
public enum StatusConditionExecution {CAST,TIME};
public enum StatusNeeded {STATUS_VALUE, SKILL};

// skill
public enum SkillEffect {NONE, ADD, REMOVE};
public enum SkillType {NONE,ACTION,EMOTION};
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
