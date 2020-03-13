using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectOrientedDesign.DeckOfCards
{
    public class BlackJackGame
    {
        
    }

    public class BlackJackCard : Card
    {
        public BlackJackCard(int face, Suit suit) : base(face, suit)
        {
        }

        public override int GetRank()
        {
            if(Face == 14)
            {
                return 11;
            }
            else if(Face > 10)
            {
                return 10;
            }
            return Face;
        }
    }
}
