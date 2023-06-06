export interface GoogleLoginResponse {
    profileObj: {
      email: string;
      familyName: string;
      givenName: string;
      googleId: string;
      imageUrl: string;
      name: string;
    };
    tokenId: string;
}

export interface GoogleLoginResponseOffline {
    code: string;
}

export type GoogleResponse = GoogleLoginResponse | GoogleLoginResponseOffline;
