using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectOrientedDesign.DeckOfCards
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public class Card
    {
        public Suit Suit { get; }
        public int Face { get; }

        public Card(int face, Suit suit)
        {
            if(face < 2 || face > 14)
            {
                throw new ArgumentException("Should be in range [2,15]",paramName: nameof(face));
            }
            Face = face;
            Suit = suit;
        }

        public virtual int GetRank()
        {
            return Face;
        }

    }

    public interface IShuffler <T>
    {
        void Shuffle(List<T> list);
    }

    public class FisherYatesShuffler <T> : IShuffler<T>
    {
        public void Shuffle(List<T> list)
        {
            Random random = new Random();

            for (int i = list.Count - 1; i > 1; i--)
            {
                int rnd = random.Next(i + 1);

                T value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
        }
    }

    public class CardsDeck <T> where T : Card
    {
        private const int TotalCards = 52;

        private readonly List<T> _deck = new List<T>(TotalCards);

        private readonly IShuffler<T> _shuffler;

        private int _nextCardToDeal;
        

        public CardsDeck(IShuffler<T> shuffler)
        {
            _shuffler = shuffler;
            _nextCardToDeal = 0;
            InitDeck();
            Shuffle();
        }

        private void InitDeck()
        {           
            for (int i = 2; i <= 14; i++)
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    var card = (T)Activator.CreateInstance(typeof(T), i, suit);
                    _deck.Add(card);
                }
            }
        }

        public void Shuffle()
        {
            _nextCardToDeal = 0;
            _shuffler.Shuffle(_deck);            
        }
        

        public Card DealNextCard()
        {
            if(_nextCardToDeal >= _deck.Count)
            {
                return null;
            }

            return _deck[_nextCardToDeal++];
        }
    }
}
