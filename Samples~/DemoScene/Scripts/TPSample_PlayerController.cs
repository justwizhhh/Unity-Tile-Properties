using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using TileProperties;

namespace TileProperties
{
    public class TPSample_PlayerController : MonoBehaviour
    {
        // Public variables
        [Header("Physics Settings")]
        public float MoveSpeed;
        public float MoveAcceleration;

        [Space(10)]
        [Header("Footstep Particle Settings")]
        public bool IsDebugTestActive;
        public float DebugTestFrequency;
        public string DebugTestPropertyName;

        // Private variables
        private bool is_moving;
        private Vector2 move_direction;

        private TilePropertiesManager current_tile_properties;
        private TPSample_TestFunctions test_functions;
        private float debug_test_timer;

        // Component references
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            current_tile_properties = FindFirstObjectByType<TilePropertiesManager>();
            test_functions = FindFirstObjectByType<TPSample_TestFunctions>();
        }

        // ------------------------------------
        //
        // Input functions
        //
        // ------------------------------------

        public void OnMovement(InputValue value)
        {
            Vector2 dir = value.Get<Vector2>();
            move_direction = dir;

            is_moving = move_direction != Vector2.zero;
        }

        public void OnQuit(InputValue value)
        {
            if (value.isPressed)
            {
                Application.Quit();
            }
        }

        // ------------------------------------
        //
        // Player movement functions
        //
        // ------------------------------------

        private void FixedUpdate()
        {
            if (is_moving)
            {
                if (rb.linearVelocity.sqrMagnitude < MoveSpeed)
                {
                    rb.AddForce(move_direction * MoveAcceleration * Time.fixedDeltaTime);
                }
            }
        }

        private void Update()
        {
            if (is_moving && IsDebugTestActive)
            {
                TriggerDebugTest();
            }
            else
            {
                debug_test_timer = 0;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                current_tile_properties.AddTileProperty("GrassTiles", "RuntimeAdded", 1.0f);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                current_tile_properties.RemoveTileProperty("GrassTiles", "RuntimeAdded");
            }
        }

        private void TriggerDebugTest()
        {
            debug_test_timer += Time.deltaTime;
            if (debug_test_timer > DebugTestFrequency)
            {
                if (test_functions.FloorTilemap != null)
                {
                    // Check the current position for a tile, to see if there are any property variables attached to it
                    TileBase current_tile = test_functions.FloorTilemap.GetTile(test_functions.FloorTilemap.WorldToCell(transform.position));
                    if (current_tile != null)
                    {
                        float tile_property = (float)current_tile_properties.GetTileProperty(current_tile, DebugTestPropertyName, typeof(float), false, false);
                        if (tile_property != 0)
                        {
                            Debug.Log(DebugTestPropertyName + ": " + tile_property + " (" + current_tile.name + ")");
                            debug_test_timer = 0;
                        }
                    }
                }
            }
        }
    }
}
