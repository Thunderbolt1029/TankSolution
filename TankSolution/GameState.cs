using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSolution
{
    enum MainStateType
    {
        InMainMenu,
        InGame,
    }

	enum SubStateType 
	{
		Paused,
        LevelSelectMenu,
		InOptionMenu,
	}

    struct GameState
    {
        public MainStateType MainState;
        public SubStateType? SubState;

        public GameState(MainStateType MainState, SubStateType SubState)
        {
            this.MainState = MainState;
            this.SubState = SubState;
        }

		public GameState(MainStateType MainState) 
		{
            this.MainState = MainState;
            SubState = null;
		}
    }
}
