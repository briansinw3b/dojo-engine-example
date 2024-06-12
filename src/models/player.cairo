use starknet::ContractAddress;

#[derive(Copy, Drop, Serde)]
#[dojo::model]
struct Player {
    #[key]
    player: ContractAddress,
    character: Character,
    score: u64
}

#[derive(Serde, Copy, Drop, Introspect)]
enum Character {
    Horseman,
    Magician,
}

impl CharacterIntoFelt252 of Into<Character, felt252> {
    fn into(self: Character) -> felt252 {
        match self {
            Character::Horseman => 1,
            Character::Magician => 2
        }
    }
}

