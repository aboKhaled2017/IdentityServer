const JsAppSettings = {
    baseAddress:"https://abokhaled.io:1201",
    client_id: "JsAppClient_id",
    get redirect_uri() {
        return encodeURIComponent(`${this.baseAddress}/SignIn`);
    },
    response_type: encodeURIComponent("id_token token"),
    scope: encodeURIComponent(`openid ApiOne`),
    get nounce() {
        return encodeURIComponent(`${this._createNounce()}`)
    },
    get state() {
        return encodeURIComponent(`${this._createState()}`)
    },
    _createNounce() {
        return "nounceValue_fffffffffffffffffffffffffffffff";
    },
    _createState() {
        return "createnounceSessionMoreAbitLonger_ddddddddddddddddddddddddddddddddddddddddd";
    },
    get getAuthorizationEndPoint() {
        var authUrl = `/connect/authorize/callback`;
        authUrl += `?client_id=${this.client_id}`;
        authUrl += `&redirect_uri=${this.redirect_uri}`;
        authUrl += `&response_type=${this.response_type}`;
        authUrl += `&scope=${this.scope}`;
        authUrl += `&nounce=${this.nounce}`;
        authUrl += `&state=${this.state}`;
        return encodeURI(authUrl);
    }
}
var signIn = () => {
    console.log(JsAppSettings.getAuthorizationEndPoint);
    window.location.href = `${JsAppSettings.baseAddress}/Auth/Login?ReturnUrl${JsAppSettings.getAuthorizationEndPoint}`
}