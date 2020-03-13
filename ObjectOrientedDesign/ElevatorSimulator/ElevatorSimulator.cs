using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectOrientedDesign.ElevatorSimulator
{
    public interface IElevatorCommand
    {
        void Execute();
    }

    public interface IElevator
    {
        void GoToFloor(int floor);
    }

    public class GoToFloorCommand : IElevatorCommand
    {
        private readonly int _targetFloor;

        private IElevator _elevator;

        public GoToFloorCommand(int targetFloor, IElevator elevator)
        {
            //TODO: check if args are valid

            _targetFloor = targetFloor;
            _elevator = elevator;
        }

        public void Execute()
        {
            _elevator.GoToFloor(_targetFloor);
        }
    }

    public class YandexElevator : IElevator
    {
        private double _elevatorSpeedMetersInSeconds;
        private double _doorsOpenCloseTimeInSeconds;
        private IBuilding _building;
        private IOutputService _outputService;

        public int CurrentFloor { get; set; }
        public string Name { get; set; }
        public YandexElevator(double elevatorSpeedMetersInSeconds, double doorsOpenCloseTimeInSeconds, IBuilding building, IOutputService outputService, string name)
        {
            //TODO: check if args are valid

            _building = building;
            _elevatorSpeedMetersInSeconds = elevatorSpeedMetersInSeconds;
            _doorsOpenCloseTimeInSeconds = doorsOpenCloseTimeInSeconds;
            _outputService = outputService;
            CurrentFloor = 1;
            Name = name;
        }
        public void GoToFloor(int targetFloor)
        {
            if(targetFloor < _building.GetMinFloor() || targetFloor > _building.GetMaxFloor())
            {
                _outputService.DisplayMessage($"floor {targetFloor} is not valid in this building");
                return;
            }

            bool goUp = false;
            if(targetFloor > CurrentFloor)
            {
                goUp = true;
            }

            while(CurrentFloor != targetFloor)
            {
                if(goUp)
                {
                    MoveOneFloorUp();
                }
                else
                {
                    MoveOneFloorDown();
                }
                _outputService.DisplayMessage($"Elevator {Name} is on {CurrentFloor} floor.");
            }

            OpenDoors();
            _outputService.DisplayMessage($"Elevator {Name}: Doors are opened");

            //TODO
            // Task.Delay(stayOpenedDelay);

            CloseDoors();
            _outputService.DisplayMessage($"Elevator {Name}: Doors are closed");

        }

        private void MoveOneFloorUp()
        {
            Task.Delay(GetTimeToMoveOneFloor()).Wait();

            CurrentFloor++;
        }

        private void MoveOneFloorDown()
        {
            Task.Delay(GetTimeToMoveOneFloor()).Wait();

            CurrentFloor--;
        }

        private TimeSpan GetTimeToMoveOneFloor()
        {
            double timeInSeconds = _building.GetFloorHeight() / _elevatorSpeedMetersInSeconds;
            return TimeSpan.FromSeconds(timeInSeconds);
        }

        private void OpenDoors ()
        {
            var delay = TimeSpan.FromSeconds(_doorsOpenCloseTimeInSeconds);
            Task.Delay(delay).Wait();
        }

        private void CloseDoors()
        {
            var delay = TimeSpan.FromSeconds(_doorsOpenCloseTimeInSeconds);
            Task.Delay(delay).Wait();
        }

    }

    public interface IBuilding
    {
        double GetFloorHeight();
        int GetMinFloor();
        int GetMaxFloor();
    }

    public class YandexBuilding : IBuilding
    {
        private readonly double _floorHeight;
        private readonly int _minFloor;
        private readonly int _maxFloor;

        public YandexBuilding(double floorHeight, int minFloor, int maxFloor)
        {
            //TODO: check if args are valid

            _floorHeight = floorHeight;
            _minFloor = minFloor;
            _maxFloor = maxFloor;
        }

        public double GetFloorHeight()
        {
            return _floorHeight;
        }

        public int GetMinFloor()
        {
            return _minFloor;
        }

        public int GetMaxFloor()
        {
            return _maxFloor;
        }
    }

    public interface IOutputService
    {
        void DisplayMessage(string message);
    }

    public class SimpleOutputService : IOutputService
    {
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }


    public class ElevatorSimulator
    {
        const int MinFloor = 1;
        Queue<IElevatorCommand> _commandsQueue;
        IElevator _mainElevator;
        IBuilding _mainBuilding;
        IOutputService _outputService;
        CancellationToken _cancellationToken;

        public ElevatorSimulator(int floors, double floorHeightInMeters, double elevatorSpeedMetersInSeconds, double periodBetweenOpenAndCloseDoorsInSeconds)
        {
            _outputService = new SimpleOutputService();

            //TODO: check if args are valid
            if (floors < 5 || floors > 20)
            {
                _outputService.DisplayMessage($"param {nameof(floors)} should be in range [5,20]");
            }
            if(floorHeightInMeters <= 0 )
            {
                _outputService.DisplayMessage($"param {nameof(floorHeightInMeters)} should be greater than 0");
            }
            if (periodBetweenOpenAndCloseDoorsInSeconds <= 0)
            {
                _outputService.DisplayMessage($"param {nameof(periodBetweenOpenAndCloseDoorsInSeconds)} should be greater than 0");
            }
            if (elevatorSpeedMetersInSeconds <= 0)
            {
                _outputService.DisplayMessage($"param {nameof(periodBetweenOpenAndCloseDoorsInSeconds)} should be greater than 0");
            }

            //TODO: extract to Abstract Factory and Use Dependency injection                
            _mainBuilding = new YandexBuilding(floorHeightInMeters, MinFloor, floors);
            _mainElevator = new YandexElevator(elevatorSpeedMetersInSeconds, periodBetweenOpenAndCloseDoorsInSeconds, _mainBuilding, _outputService, "Elevator 1");
        }

        public void Start()
        {   
            _commandsQueue = new Queue<IElevatorCommand>();

            _cancellationToken = new CancellationToken(false);
            Task.Run(() => DoWork(), _cancellationToken);
          }

        public void CallElevatorFromFloor(int floor)
        {
            IElevatorCommand elevatorCommand = new GoToFloorCommand(floor, _mainElevator);
            _commandsQueue.Enqueue(elevatorCommand);
        }

        public void PressButtonInElevator(int floor)
        {
            IElevatorCommand elevatorCommand = new GoToFloorCommand(floor, _mainElevator);
            _commandsQueue.Enqueue(elevatorCommand);
        }


        private void DoWork()
        {
            while(true)
            {
                if(_commandsQueue.Count > 0)
                {
                    var command = _commandsQueue.Dequeue();
                    command.Execute();
                }
                
            }
        }
    }
}
