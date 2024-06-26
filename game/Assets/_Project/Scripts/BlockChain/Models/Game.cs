// Generated by dojo-bindgen on Wed, 12 Jun 2024 06:17:57 +0000. Do not modify this file manually.
using System;
using Dojo;
using Dojo.Starknet;

namespace Game
{
    // Type definition for `dojo_starter::models::game::GameStatus` enum
    public abstract record GameStatus() {
        public record Lobby() : GameStatus;
        public record InProgress() : GameStatus;
        public record Lost() : GameStatus;
        public record Won() : GameStatus;
    }


    // Model definition for `dojo_starter::models::game::Game` model
    public class Game : ModelInstance {
        [ModelField("player")]
        public FieldElement player;

        [ModelField("entityId")]
        public uint entityId;

        [ModelField("status")]
        public GameStatus status;

        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {
        }
    }
            
}