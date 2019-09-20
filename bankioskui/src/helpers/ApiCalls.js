class ApiCalls {
    
    constructor() {
        this.bankiosk = process.env.REACT_APP_BASE_CORE_URL;
        
        // instantiate endpoints
        this._photoEndPoint = "/api/photo";
        this._ccAuthEndPoint = "/api/ccauth";
        this._actionsListEndPoint = "/api/action";

        // for audits
        this._auditActionEndPoint = "/api/audit/auditAction";
        this._auditAuthenticateEndPoint = "/api/audit/auditAuthentication";

        // For tasks
        this._userAppointments = "/api/banktask/appointments";
        this._userDocuments = "/api/banktask/documents";
    }

    photoEndPoint() {
        return (`${this.bankiosk}${this._photoEndPoint}`);
    }

    ccAuthEndPoint(id) {
        return (`${this.bankiosk}${this._ccAuthEndPoint}?id=${id}`);
    }

    actionsListEndPoint() {
        return (`${this.bankiosk}${this._actionsListEndPoint}`);
    }

    auditActionEndPoint(param) {
        return (`${this.bankiosk}${this._auditActionEndPoint}/${param}`);
    }

    auditAuthenticateEndPoint(param) {
        return (`${this.bankiosk}${this._auditAuthenticateEndPoint}/${param}`);
    }

    userAppointments(param) {
        return (`${this.bankiosk}${this._userAppointments}/${param}`); 
    }

    userDocuments(param) {
        return (`${this.bankiosk}${this._userDocuments}/${param}`); 
    }

    async Get(url) {
        return fetch(url, {
            headers: {}
        });
    }

    async Post(url, jsonBody) {
        return fetch(url,
        {
            method: "POST",
            body: JSON.stringify(jsonBody),
            headers: {
                'Content-Type': 'application/json'
            }
        })
    }

    async Put(url, jsonBody) {
        return fetch(url,
        {
            method: "PUT",
            body: JSON.stringify(jsonBody),
            headers: {
                'Content-Type': 'application/json'
            }
        })
    }

    async PostImage(url, img) {
        return fetch(url,
        {
            method: "POST",
            body: img,
            processData: false,
            headers: {
                'Content-Type': 'application/octet-stream'
            }
        })
    }
}

export default ApiCalls;