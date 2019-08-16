
// Item
public enum ValueSetter {PERCENT, VALUE};
public enum ItemSkillType {NONE, LEARN, USE};
public enum ItemVariableType {NONE, SET, REMOVE};
public enum ItemDropType {NONE, ITEM, EQUIPMENT, SKILL};
public enum ColumnFill {VERTICAL, HORIZONTAL};
public enum ItemType{INVENTORY, QUEST, GIFT}


// status values
public enum StatusValueType {NORMAL, CONSUMABLE, EXPERIENCE};

// status effect
public enum StatusEffectEnd {NONE, TIME};
public enum StatusConditionExecution {CAST,TIME};
public enum StatusNeeded {STATUS_VALUE, SKILL};

// skill
public enum SkillEffect {NONE, ADD, REMOVE};
public enum SkillType {NONE,ACTION,EMOTION};
public enum Direction {R, D, L, U};

//Movement
public enum MovementType{WALK = 0,RUN = 1,CRAW = 2,JUMP = 3,FLY = 4};

// formula
public enum SimpleOperator {ADD, SUB, SET};
public enum FormulaChooser {VALUE, STATUS, RANDOM};
public enum StatusOrigin {USER, TARGET};
public enum FormulaOperator {DONE, ADD, SUB, MULTIPLY, DIVIDE, MODULO, POWER_OF, LOG};
public enum Rounding {NONE, CEIL, FLOOR, ROUND};
public enum ValueCheck {EQUALS, LESS, GREATER};

// event
public enum EventStartType {INTERACT, AUTOSTART, TRIGGER_ENTER};
