using System;
using System.Collections.Generic;

namespace OrX.chase
{
	interface IDetection
	{
		void UpdateMap (List<OrXchaseContainer> collection);
		void Debug();
	}
}

