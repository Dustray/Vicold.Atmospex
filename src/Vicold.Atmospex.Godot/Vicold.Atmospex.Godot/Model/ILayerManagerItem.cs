
using Godot;

namespace RMIAS.VISION.Model
{
	internal interface ILayerManagerItem
	{

		string ID { get;  }
		bool IsSystemLayer{get;set;}
		void SetItemVisible(bool isShow);
		void SetMessage(string id, string name);
	}
}
