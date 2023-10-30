using UnityEngine;
using UnityEngine.UI;

namespace Source.UI
{
    public class UIStepCount : MonoBehaviour
    {
        [SerializeField] private Text _text;

        private Player _player;
        private int _stepCompleteCount;
        
        public void Initialize(Player player)
        {
            _player = player;
            _player.moveNextStep += MoveNextStep;
            _player.playerDeathNotify += PlayerDeathNotify;
            _text.text = _stepCompleteCount.ToString();
        }

        private void MoveNextStep()
        {
            _stepCompleteCount++;
            _text.text = _stepCompleteCount.ToString();
        }

        private void PlayerDeathNotify()
        {
            _stepCompleteCount = 0;
            _text.text = _stepCompleteCount.ToString();
        }

        private void OnDestroy()
        {
            _player.moveNextStep -= MoveNextStep;
            _player.playerDeathNotify -= PlayerDeathNotify;
        }
    }
}