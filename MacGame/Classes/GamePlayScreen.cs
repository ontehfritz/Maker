using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;


namespace maker {
    public class GamePlayScreen : GameScreen {

        public Dictionary<string, Objekt> _objekts; 
        bool _jump;

        public GamePlayScreen () : base() {
            _objekts = new Dictionary<string, Objekt>();
        }

        protected Objekt Collided(Objekt o)
        {
            foreach(KeyValuePair<string, Objekt> kvp in _objekts){
                if(kvp.Value.InScreen() && !kvp.Value.Equals(o) && kvp.Value.Collidable){
                    if(Utility.BoundingCollision(o, kvp.Value)){
                        return kvp.Value;
                    }
                }
            }
            return null;
        }


        public override void LoadContent () {
            MacGame _game = (MacGame)ScreenManager.Game;
            _objekts.Add("bg1", new Objekt(new Sprite(_game.Content, "Background01"), 
                                           _game.spriteBatch, _game.graphics, _game.camera,false));
            _objekts.Add("bg2", new Objekt(new Sprite(_game.Content, "Background02"), 
                                           _game.spriteBatch, _game.graphics, _game.camera, false));
            _objekts.Add("bg3", new Objekt(new Sprite(_game.Content, "Background03"),
                                           _game.spriteBatch, _game.graphics, _game.camera, false));
            _objekts.Add("bg4", new Objekt(new Sprite(_game.Content, "Background05"), 
                                           _game.spriteBatch, _game.graphics, _game.camera, false));
            
            _objekts["bg1"].Scale = 3.0f;
            _objekts["bg2"].Scale = 3.0f;
            _objekts["bg3"].Scale = 3.0f;
            _objekts["bg4"].Scale = 3.0f;
            
            for(int i = 0; i < 32; i++){
                _objekts.Add("tile" + i.ToString(), 
                             new Tile(new Sprite(_game.Content, "Ground"),
                         _game.spriteBatch,
                         _game.graphics,
                         _game.camera, 
                         true));
                
                _objekts["tile" + i.ToString()].Position = new Vector2(i * 32,650);
                ((Tile)_objekts["tile" + i.ToString()]).SolidTop = true;
            }
            
            
            for(int i = 0; i < 32; i++){
                _objekts.Add("tileA" + i.ToString(), 
                             new Tile(new Sprite(_game.Content, "Ground"),
                         _game.spriteBatch,
                         _game.graphics,
                         _game.camera, true));
                
                _objekts["tileA" + i.ToString()].Position = new Vector2((i + 30) * 32,550);
                ((Tile)_objekts["tileA" + i.ToString()]).SolidTop = true;
            }
            
            _objekts.Add("hero", 
                         new Player(new Sprite(_game.Content, "maker_walk", 1, 4),
                       _game.spriteBatch,
                       _game.graphics,
                       _game.camera));
            
            _objekts["hero"].Position = new Vector2(10, 100); 
            _objekts["bg1"].Position = new Vector2(0,-100);
            _objekts["bg2"].Position = new Vector2(_objekts["bg1"].Position.X + _objekts["bg1"].Size.Width, -100);
            _objekts["bg3"].Position = new Vector2(_objekts["bg2"].Position.X + _objekts["bg2"].Size.Width, -100);
            _objekts["bg4"].Position = new Vector2(_objekts["bg3"].Position.X + _objekts["bg3"].Size.Width, -100);
            
            _objekts.Add("mouse-pointer", 
                         new Objekt(new Sprite(_game.Content, "mouse-pointer"), 
                       _game.spriteBatch, _game.graphics, _game.camera, false));
            
            _objekts["mouse-pointer"].Position = new Vector2(0,0);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
            if (IsActive && !coveredByOtherScreen)
            {
                Camera camera = ((MacGame)ScreenManager.Game).camera;

                Player player = ((Player)_objekts["hero"]);
                // For Mobile devices, this logic will close the Game when the Back button is pressed
                
                if(Mouse.GetState().LeftButton == ButtonState.Pressed){
                    int x = Mouse.GetState().X + (int)camera.Position.X;
                    int y = Mouse.GetState().Y + (int)camera.Position.Y;

                    if(!_objekts.ContainsKey("tileB" + x.ToString() + y.ToString()))
                    {
                        Objekt newO = new Tile(new Sprite(ScreenManager.Game.Content, "Ground"),
                                               ScreenManager.SpriteBatch,
                                               ((MacGame)ScreenManager.Game).graphics,
                                               camera, true);
                        
                        newO.Position = new Vector2(x,y);
                        
                        if(Collided(newO) == null)
                        {
                            _objekts.Add("tileB" + x.ToString() + y.ToString(), 
                                        new Tile(new Sprite(ScreenManager.Game.Content, "Ground"),
                                     ScreenManager.SpriteBatch,
                                     ((MacGame)ScreenManager.Game).graphics,
                                     camera, true));
                            
                            _objekts["tileB" + x.ToString() + 
                                    y.ToString()].Position = new Vector2(x,y);
                        }
                    }
                }
                
                if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Tab)){
                    TileScreen tileScreen = new TileScreen();
                    tileScreen.DialogueText = " ";
                    tileScreen.TitleText = "Maker";
                    tileScreen.BackText = "This is back text";
                    ScreenManager.AddScreen(tileScreen);
                }

                if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left)){
                    player.playerStates["LEFT"] = true; 
                    player.playerStates["RIGHT"] = false; 
                }
                
                if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right)){
                    player.playerStates["RIGHT"] = true; 
                    player.playerStates["LEFT"] = false;
                }
                
                if(!_jump)
                {
                    if(Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Space)){
                        if(player.playerStates["FALL"] == false){
                            player.playerStates["JUMP"] = true; 
                            _jump = true;
                        }
                    }
                }
                
                if(_jump)
                {
                    if(Keyboard.GetState(PlayerIndex.One).IsKeyUp(Keys.Space)){
                        if(player.playerStates["JUMP"] == true){
                            player.playerStates["JUMP"] = false; 
                        }
                        _jump = false;
                    }
                }
                
                if(Collided (player) != null){
                    player.playerStates["FALL"] = false;
                    //System.Console.WriteLine("Collided:" + kvp.Key);
                }
                else{
                    if(player.playerStates["JUMP"] == false){
                        player.playerStates["FALL"] = true;
                    }
                }
                
                player.Actions();
                
                _objekts["mouse-pointer"].Position = 
                    new Vector2(Mouse.GetState().X + (int)camera.Position.X, 
                                Mouse.GetState().Y + (int)camera.Position.Y); 
                
                if(player.Position.X > ((MacGame)ScreenManager.Game).graphics.GraphicsDevice.DisplayMode.Width / 2)
                    camera.Position = 
                        new Vector2(player.Position.X - ((MacGame)ScreenManager.Game).graphics.GraphicsDevice.DisplayMode.Width / 2,0);
                
                // TODO: Add your update logic here 
                //screenManager.Update(gameTime);



                //Session.Update(gameTime);
            }
        }



        public override void Draw (GameTime gameTime) {
            //graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            

            
            foreach(KeyValuePair<string, Objekt> kvp in _objekts){
                if(kvp.Value.InScreen()){
                    kvp.Value.Draw();
                }
            }
            
            //_objekts["mouse-pointer"].Draw();
            spriteBatch.End();
        }
    }
}
