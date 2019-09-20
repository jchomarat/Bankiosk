import ApiCalls from '../helpers/ApiCalls';

class Audit {
    
    async ActionPerformed(actionName) {
        let api = new ApiCalls();
        await api.Post(api.auditActionEndPoint(actionName), {});
        return true;
    }

    async AuthenticationPerformed(mode) {
        let api = new ApiCalls();
        await api.Post(api.auditAuthenticateEndPoint(mode), {});
        return true;
    }
}

export default Audit;