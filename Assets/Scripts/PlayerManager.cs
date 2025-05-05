using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int activeMovePosIndex;
    private List<Vector3[]> movePos;
    private float ticks;
    private bool canWalk;

    private const float WALKING_SPEED = 5f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (canWalk)
        {
            Vector3 start = new Vector3(movePos[activeMovePosIndex][0].x, movePos[activeMovePosIndex][0].y, 0);
            Vector3 destination = new Vector3(movePos[activeMovePosIndex][1].x, movePos[activeMovePosIndex][1].y, 0);

            float totalDistance = Vector3.Distance(start, destination);
            ticks += (Time.deltaTime * WALKING_SPEED) / totalDistance;
            ticks = Mathf.Min(ticks, 1);

            transform.localPosition = Vector3.Lerp(start, destination, ticks);

            spriteRenderer.flipX = destination.x - start.x < 0;

            if (transform.localPosition == destination)
            {
                activeMovePosIndex++;
                activeMovePosIndex = activeMovePosIndex > movePos.Count - 1 ? -1 : activeMovePosIndex;

                if(activeMovePosIndex == -1)
                {
                    Stay();
                }
                else
                {
                    ticks = 0;
                }
            }
        }
    }

    internal void SetPosition(Vector3 position)
    {
        transform.localPosition = new Vector3(position.x, position.y, 0);
    }

    internal void Move(List<Vector3[]> movePos)
    {
        this.movePos = movePos;

        activeMovePosIndex = 0;
        ticks = 0;

        animator.SetBool("Walk", true);

        canWalk = true;
    }

    private void Stay()
    {
        animator.SetBool("Walk", false);

        canWalk = false;
    }
}