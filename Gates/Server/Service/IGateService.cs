using Gates.Shared.Data;

namespace Gates.Server.Service
{
    public interface IGateService
    {
        public Task<List<GateModel>> GetGate(string webhookState);
        public bool AddGate(GateModel model);
        public bool RemoveGate(GateModel model);
        public Task<bool> CheckGate(GateModel model);
        public void UpdateGate(GateModel model);
        public void OpenGate(GateModel model);
        public void CloseGate(GateModel model);
        Task<List<GateModel>> GetGateByAppId(int appId);
        public void UpdateIsWaiting(GateModel model);
        public void ResetWaiting(GateModel model);
        Task<GateModel> GetGate(string name, string @namespace, string webhookState);
    }
}
