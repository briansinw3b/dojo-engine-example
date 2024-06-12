use starknet::ContractAddress;

#[derive(Copy, Drop, Serde)]
#[dojo::model]
struct Skill {
    #[key]
    entityId: ContractAddress,
    #[key]
    gameId: u32,
    attack: u16,
    strongAttack: u16,
    healing: u16
}

#[derive(Serde, Copy, Drop, Introspect)]
enum SkillType {
    Attack,
    StrongAttack,
    Healing
}

impl SkillTypeIntoFelt252 of Into<SkillType, felt252> {
    fn into(self: SkillType) -> felt252 {
        match self {
            SkillType::Attack => 1,
            SkillType::StrongAttack => 2,
            SkillType::Healing => 3
        }
    }
}
