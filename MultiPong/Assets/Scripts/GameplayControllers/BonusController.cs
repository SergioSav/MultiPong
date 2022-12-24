using Assets.Scripts.Data;
using Assets.Scripts.Models;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BonusController : NetworkBehaviour
{
    private NetworkVariable<int> _modelId = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private Renderer _iconRenderer;
    private BonusModel _model;

    public void Setup(BonusModel model)
    {
        _model = model;
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            _modelId.Value = _model.Id;
            _model.NetId = NetworkObjectId;
        }
    }

    private void Start()
    {
        if (_model == null)
        {
            _model = GameSource.Instance.BonusModels
                            .Where(b => b.Id == _modelId.Value)
                            .FirstOrDefault();
        }
        _iconRenderer.material = _model.IconMaterial;
        StartCoroutine(AppearEffects());
    }

    private IEnumerator AppearEffects()
    {
        yield return new WaitForSeconds(0.5f);
        _model.CanBeCaptured = true;
    }
}
