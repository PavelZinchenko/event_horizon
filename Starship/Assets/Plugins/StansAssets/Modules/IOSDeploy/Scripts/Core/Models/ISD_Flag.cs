using UnityEngine;
using System.Collections;

namespace SA.IOSDeploy {

	[System.Serializable]
	public class Flag  {

		//Editor Use Only
		public bool IsOpen = true;

		public string Name;
		public FlagType Type;

	}

}
