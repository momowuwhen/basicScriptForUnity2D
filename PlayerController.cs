// player controller for 2D top-down / 2.5D game

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // for type 'InputValue', installed from Package Manager/Unity Registry/Input System.


// https://www.youtube.com/watch?v=7iYWpzL9GkM

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 movementInput; // X&Y (-1,1) user input directions
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f; // collid distance gap
    public ContactFilter2D movementFilter; // where collision occurs, e.g. layers
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>(); // list of found collisions

    Animator animator; // for animation state switching
    SpriteRenderer spriteRenderer; // for flipping the character's facing directions

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);
            // if blocked by collider and cannot move, try to move on the x direction first, then try to move on the y direction.
            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
                if (!success)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }
            animator.SetBool("isMoving", success); // RENAME! Depending on condition bool name in Animator !!!
        } else
        {
            animator.SetBool("isMoving", false);
        }

        if(movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        } else if(movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if(direction != Vector2.zero)
        {
            // check for potential collisions
            // public int Cast(Vector2 direction, 'Vector representing the direction to cast each Collider2D shape.'
            //                  ContactFilter2D contactFilter, 'Filter results defined by the contact filter.'
            //                  RaycastHit2D[] results, 'Array to receive results.'
            //                  float distance = Mathf.Infinity 'Maximum distance over which to cast the shape(s).');
            int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // amount to cast
            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        } else
        {   // if cannot move(blocked), then don't show moving animation
            return false;
        }
        
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }
}
