using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerControls
{
    /// <summary>
    /// Core controller for the player's Hero ship.
    /// Handles movement with screen boundary clamping, weapon systems, 
    /// and power-up absorption using Unity's New Input System.
    /// </summary>
    public class Hero : MonoBehaviour
    {
        static public Hero S; // Singleton instance for easy access

        [Header("Movement Settings")]
        public float speed = 30f;
        public float rollMult = -45f;   // Banking rotation intensity (Z-axis)
        public float pitchMult = 30f;   // Pitch rotation intensity (X-axis)
        public float boundaryRadius = 1.5f; // Keeps the ship within the visible play area

        [Header("Combat & Stats")]
        [SerializeField] private float _shieldLevel = 1f;
        public Weapon[] weapons; // Array of attached weapon slots

        public delegate void WeaponFireDelegate();
        public WeaponFireDelegate fireDelegate;
        
        // Input System variables
        private InputSystem_Actions controls; 
        private Vector2 moveInput;
        private GameObject lastTriggerGo = null; // Prevents double-triggering on same object

        void Awake()
        {
            // Set the Singleton instance
            if (S == null) {
                S = this;
            } else {
                Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
            }
            
            // Initialize the New Input System actions
            controls = new InputSystem_Actions();
            
            // Register input callbacks
            controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
            
            // Subscribe to the Fire action
            controls.Player.Fire.performed += ctx => HandleFire();

            // Auto-fill weapons array if empty in Inspector
            if (weapons == null || weapons.Length == 0) {
                weapons = GetComponentsInChildren<Weapon>();
            }
        }

        // Enable and Disable input actions with the GameObject lifecycle
        void OnEnable() {
            if (controls != null) controls.Enable();
        }

        void OnDisable() {
            if (controls != null) controls.Disable();
        }

        void Update()
        {
            // 1. Position calculation based on Input and Speed
            Vector3 pos = transform.position;
            pos.x += moveInput.x * speed * Time.deltaTime;
            pos.y += moveInput.y * speed * Time.deltaTime;

            // 2. Clamp position to keep the Hero on screen (Standard Shmup logic)
            float camHeight = Camera.main.orthographicSize;
            float camWidth = camHeight * Camera.main.aspect;

            pos.x = Mathf.Clamp(pos.x, -camWidth + boundaryRadius, camWidth - boundaryRadius);
            pos.y = Mathf.Clamp(pos.y, -camHeight + boundaryRadius, camHeight - boundaryRadius);

            transform.position = pos;

            // 3. Dynamic banking rotation for better visual feel
            transform.rotation = Quaternion.Euler(moveInput.y * pitchMult, moveInput.x * rollMult, 0);
        }

        /// <summary>
        /// Fires all active weapons. Can be extended with fire-rate limiting.
        /// </summary>
        void HandleFire()
        {
            if (fireDelegate != null) fireDelegate();
            foreach (Weapon w in weapons) {
                if (w != null) w.Fire(); 
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Get the root object to avoid multi-collider triggers
            Transform rootT = other.gameObject.transform.root;
            GameObject go = rootT.gameObject;

            if (go == lastTriggerGo) return;
            lastTriggerGo = go;

            if (go.CompareTag("Enemy")) {
                shieldLevel--;
                Destroy(go);
            } else if (go.CompareTag("PowerUp")) {
                AbsorbPowerUp(go);
            }
        }

        /// <summary>
        /// Logic for processing different PowerUp types (Shields, Weapons, Lives).
        /// </summary>
        public void AbsorbPowerUp(GameObject go)
        {
            PowerUp pu = go.GetComponent<PowerUp>();
            if (pu == null) return;

            switch (pu.type) {
                case WeaponType.life: // Use 'life' or 'health' based on your Enum definition
                    // Static increment for persistence across reloads
                    Main.lives++;
                    if (Main.S != null) Main.S.UpdateGUI();
                    break;

                case WeaponType.shield:
                    shieldLevel++;
                    break;

                default: // Handle Weapon-type PowerUps
                    if (pu.type == weapons[0].type) { // If same type, fill empty slots (Spread logic)
                        Weapon w = GetEmptyWeaponSlot();
                        if (w != null) w.SetType(pu.type);
                    } else { // If different type, clear all and set new primary weapon
                        ClearWeapons();
                        weapons[0].SetType(pu.type);
                    }
                    break;
            }
            pu.AbsorbedBy(this.gameObject);
        }

        public float shieldLevel
        {
            get { return _shieldLevel; }
            set {
                _shieldLevel = Mathf.Clamp(value, 0, 4);
                if (_shieldLevel <= 0) {
                    Destroy(this.gameObject);
                    // Notify Main script about player death
                    if (Main.S != null) Main.S.HeroDied(); 
                }
            }
        }

        Weapon GetEmptyWeaponSlot() {
            foreach (Weapon w in weapons) {
                if (w.type == WeaponType.none) return w;
            }
            return null;
        }

        void ClearWeapons() {
            foreach (Weapon w in weapons) {
                w.SetType(WeaponType.none);
            }
        }
    }
}
