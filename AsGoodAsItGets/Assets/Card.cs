using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int Id;

    private SpriteRenderer _spriteRenderer;
    private MeshRenderer _meshRenderer;
    [SerializeField] string _type;

    public string Type => _type;

    public void Initialize(int id)
    {
        Id = id;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        string test = $"{_type}/{_type[0]}{Id}";
        Sprite sprite = Resources.Load<Sprite>($"{_type}/{_type[0]}{Id}");

        _spriteRenderer.sprite = sprite;
        _spriteRenderer.enabled = false;
    }

    public void Flip()
	{
        if (_spriteRenderer.enabled)
		{
            _spriteRenderer.enabled = false;
            _meshRenderer.enabled = true;
		}
		else
		{
            _spriteRenderer.enabled = true;
            _meshRenderer.enabled = false;
        }
        
    }

}
