import ApiCalls from '../helpers/ApiCalls';
import Audit from '../helpers/Audit';

class Authenticate {
    
    async Face(blob) {
        let api = new ApiCalls();
        let response = await api.PostImage(api.photoEndPoint(), blob);
        let data = await response.json();

        if (data.status) {
            // Log it
            let audit = new Audit();
            await audit.AuthenticationPerformed("faceAPI");

            return {status: true, data: JSON.parse(data.value), funFact: null};
        }
        else {
            // No user found, try to get fun fact out of the photo to make the kiosk more "alive"
            let response = await api.PostImage(api.photoFunFactEndPoint(), blob);
            let funFact = await response.json();
            return {status: false, data: null, funFact: (response.status && funFact.status) ? JSON.parse(funFact.value) : null};
        }
    }

    async CreditCard(userId) {
        let api = new ApiCalls();
        let response = await api.Get(api.ccAuthEndPoint(userId));
        let data = await response.json();

        if(data.status) {
            // Log it
            let audit = new Audit();
            await audit.AuthenticationPerformed("cc_number");

            return {status: true, data: JSON.parse(data.value)};
        }
        else {
            return {status: false, data: null};
        }
    }
}

export default Authenticate;