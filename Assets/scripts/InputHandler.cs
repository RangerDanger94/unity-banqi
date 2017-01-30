using UnityEngine;

namespace Assets
{
    class InputHandler : MonoBehaviour
    {
        private void OnMouseDown()
        {
            int x = (int)transform.position.x;
            int y = (int)transform.position.y;

            GameManager.SelectPiece(x, y);
        }
    }
}
