using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Village_Racing
{
    class TitleScreen
    {
        Texture2D smallCloud;
        Texture2D largeCloud;
        Texture2D Hill1;
        Texture2D Hill2;
        Texture2D Background;
        Vector2 smallCloudPosition = new Vector2(200, -80);
        Vector2 largeCloudPosition = new Vector2(-100, 250);
        Vector2 Hill1Position = new Vector2(0, 545);
        Vector2 Hill2Position = new Vector2(0, 585);

        public TitleScreen(Texture2D SC, Texture2D LC, Texture2D H1, Texture2D H2, Texture2D BG)
        {
            smallCloud = SC;
            largeCloud = LC;
            Hill1 = H1;
            Hill2 = H2;
            Background = BG;
        }

        public void Update(GameTime gametime)
        {
            smallCloudPosition.X += 0.05f;
            if (smallCloudPosition.X >= 1024)
            {
                smallCloudPosition.X = -408;
            }

            largeCloudPosition.X += 0.14f;
            if (largeCloudPosition.X >= 1024)
            {
                largeCloudPosition.X = -673;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, new Rectangle(0, 0, 1024, 768), Color.White);
            spriteBatch.Draw(smallCloud, new Rectangle((int)smallCloudPosition.X, (int)smallCloudPosition.Y, 408, 306), Color.White);
            spriteBatch.Draw(largeCloud, new Rectangle((int)largeCloudPosition.X, (int)largeCloudPosition.Y, 673, 223), Color.White);
            spriteBatch.Draw(Hill2, new Rectangle((int)Hill1Position.X, (int)Hill1Position.Y, 1064, 223), Color.White);
            spriteBatch.Draw(Hill1, new Rectangle((int)Hill2Position.X, (int)Hill2Position.Y, 1064, 223), Color.White);
        }
    }
}
