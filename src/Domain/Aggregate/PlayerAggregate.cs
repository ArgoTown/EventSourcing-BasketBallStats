using BasketballStats.Domain.Events;
using BasketballStats.Domain.Models;

namespace BasketballStats.Domain.Aggregate
{
    public sealed class PlayerAggregate : BaseAggregate<Player>
    {
        public PlayerAggregate(Player player)
        {
            State = player;
        }

        public void AddStatistic(NegativeStatistic statistic)
        {
            var @event = new NegativeEventHappened(statistic, State.GameId, State.TeamId, State.Id, Guid.NewGuid());
            Raise(@event);
            State.Apply(@event);
        }

        public void AddStatistic(PositiveStatistic statistic)
        {
            var @event = new PositiveEventHappened(statistic, State.GameId, State.TeamId, State.Id, Guid.NewGuid());
            Raise(@event);
            State.Apply(@event);
        }

        public PlayerStats TotalStats()
        {
            var totalPoints = Convert.ToInt16(State.MadeThreePoints * 3 + State.MadeTwoPoints * 2 + State.MadeFreeThrows * 1);

            var freeThrows = Convert.ToDecimal(State.MissedFreeThrows) + Convert.ToDecimal(State.MadeFreeThrows);
            var freeThrowPercentage = freeThrows.Equals(0) ? 0 : Convert.ToDecimal(State.MadeFreeThrows) / freeThrows * 100;

            var twoPointsMade = Convert.ToDecimal(State.MissedTwoPoints) + Convert.ToDecimal(State.MadeTwoPoints);
            var twoPointPercentage = twoPointsMade.Equals(0) ? 0 : Convert.ToDecimal(State.MadeTwoPoints) / twoPointsMade * 100;

            var threePointsMade = Convert.ToDecimal(State.MissedThreePoints) + Convert.ToDecimal(State.MadeThreePoints);
            var threePointPercentage = threePointsMade.Equals(0) ? 0 : Convert.ToDecimal(State.MadeThreePoints) / threePointsMade * 100;

            return new PlayerStats
            {
                TotalPoints = totalPoints,
                FreeThrowPercentage = Convert.ToInt16(Math.Round(freeThrowPercentage)),
                TwoPointPercentage = Convert.ToInt16(Math.Round(twoPointPercentage)),
                ThreePointPercentage = Convert.ToInt16(Math.Round(threePointPercentage))
            };
        }

        protected override void When(IEvent evt)
        {
            switch (evt)
            {
                case PositiveEventHappened positiveAction:
                    State.Apply(positiveAction);
                    break;
                case NegativeEventHappened negativeAction:
                    State.Apply(negativeAction);
                    break;
                default:
                    throw new ArgumentException("Event not supported.", nameof(evt));
            }
        }
    }
}
