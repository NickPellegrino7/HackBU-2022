using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int Id = -1;
    public GameObject image;

    // Values for up/down movement animation
    private float _originalHeight;
    private float _movementOffset;
    private float _targetOffset;
    private int _lastHitCounter;
    private bool _hitSummit;

    private SpriteRenderer _spriteRenderer;
    private MeshRenderer _meshRenderer;
    [SerializeField] string _type;

    public string Type => _type;

    void Awake() {
        _originalHeight = image.transform.localPosition.y;
        _movementOffset = 0.05f;
        _targetOffset = 1f;
        _hitSummit = true;
    }

    public void Initialize(int id) {
        Id = id;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        string test = $"{_type}/{_type[0]}{Id}";
        Sprite sprite = Resources.Load<Sprite>($"{_type}/{_type[0]}{Id}");
        print($"{_type}/{_type[0]}{Id}");

        _spriteRenderer.sprite = sprite;
        _spriteRenderer.enabled = false;
    }

    public void Flip()
    {
        if (Id != -1) {
            if (_spriteRenderer.enabled) {
                _spriteRenderer.enabled = false;
                _meshRenderer.enabled = true;
            } else {
                _spriteRenderer.enabled = true;
                _meshRenderer.enabled = false;
            }
        }
    }

    public void AnimateUp() {
        if (image.transform.localPosition.y < (_originalHeight + _targetOffset)) {
            image.transform.localPosition = new Vector3(image.transform.localPosition.x, image.transform.localPosition.y + _movementOffset, image.transform.localPosition.z);
            _hitSummit = false;
        } else {
            image.transform.localPosition = new Vector3(image.transform.localPosition.x, _originalHeight + _targetOffset, image.transform.localPosition.z);
            _hitSummit = true;
        }
    }

    public void HoverOver() {
        _lastHitCounter = 3; // Freeze Update() until this is no longer being called
        AnimateUp();
    }

    public void AnimateDown() {
        if (image.transform.localPosition.y > _originalHeight) {
            image.transform.localPosition = new Vector3(image.transform.localPosition.x, image.transform.localPosition.y - _movementOffset, image.transform.localPosition.z);
        } else {
            image.transform.localPosition = new Vector3(image.transform.localPosition.x, _originalHeight, image.transform.localPosition.z);
        }
    }

    void Update() {
        if (_lastHitCounter > 0) {
            _lastHitCounter -= 1; // Try to unfreeze self, succeed when HoverOver isn't being called anymore
        } else {
            if (!_hitSummit) { AnimateUp(); }
            else { AnimateDown(); }
        }
    }
}
