using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace QuickScript
{
	[AddComponentMenu("Quick Script/Actions/Restart Scene")]
	public class RestartScene : MonoBehaviour
	{
		public UnityEvent onBeforeRestart = new UnityEvent();

		public void Restart()
		{
			onBeforeRestart.Invoke();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}