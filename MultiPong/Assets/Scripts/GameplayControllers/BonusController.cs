using Assets.Scripts.Models;
using System.Collections;
using UnityEngine;

public class BonusController : MonoBehaviour, IPosition
{
    [SerializeField] private Renderer _iconRenderer;
    private BonusModel _model;
    private bool _canBeCaptured;

    public float PosX => transform.position.x;
    public float PosY => transform.position.y;

    public bool CanBeCaptured => _canBeCaptured;

    public BonusModel Model=> _model;

    public void Setup(BonusModel model)
    {
        _model = model;
    }

    public void HandleCapture()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        _iconRenderer.material = _model.IconMaterial;
        StartCoroutine(AppearEffects());
    }

    private IEnumerator AppearEffects()
    {
        yield return new WaitForSeconds(0.5f);
        _canBeCaptured = true;
    }
}
