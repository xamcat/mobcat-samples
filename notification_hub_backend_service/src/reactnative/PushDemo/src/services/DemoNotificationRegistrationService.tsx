export default class DemoNotificationService {
    constructor(
        readonly apiUrl: string,
        readonly apiKey: string) {

    }

    async registerAsync(request: any): Promise<Response> {
        const method = 'PUT';
        const registerApiUrl = `${this.apiUrl}/notifications/installations`;
        const result = await fetch(registerApiUrl, {
            method: method,
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                'apiKey': this.apiKey
            },
            body: JSON.stringify(request)
        });

        this.validateResponse(registerApiUrl, method, request, result);
        return result;
    }

    async deregisterAsync(deviceId: string): Promise<Response> {
        const method = 'DELETE';
        const deregisterApiUrl = `${this.apiUrl}/notifications/installations/${deviceId}`;
        const result = await fetch(deregisterApiUrl, {
            method: method,
            headers: {
                Accept: 'application/json',
                'Content-Type': 'application/json',
                'apiKey': this.apiKey
            }
        });

        this.validateResponse(deregisterApiUrl, method, null, result);
        return result;
    }

    private validateResponse(requestUrl: string, method: string, requestPayload: any, response: Response) {
        console.log(`Request: ${method} ${requestUrl} => ${JSON.stringify(requestPayload)}\nResponse: ${response.status}`);
        if (!response || response.status != 200) {
            throw `HTTP error ${response.status}: ${response.statusText}`;
        }
    }
}