using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectOrientedDesign.DeckOfCards;

namespace UnitTestProject1
{
    [TestClass]
    public class DeckOfCardsTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Exception should be thrown for invalid (-1) input")]
        public void CardConstructor_WhenInvalidInput_ShouldThrhow()
        {
            Card card = new Card(-1, Suit.Clubs);
        }
    }
}
