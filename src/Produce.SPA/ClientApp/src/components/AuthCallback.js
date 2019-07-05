import React, {PureComponent} from 'react';
import * as Oidc from "oidc-client";
import {Redirect} from "react-router-dom";

export class AuthCallback extends PureComponent {
    componentDidMount() {
        new Oidc.UserManager({response_mode: "query"})
            .signinRedirectCallback()
            .then(() => {
                return <Redirect to="/" push />
            }).catch(function (e) {
            console.error(e);
        });
    }

    render() {
        return <div>Redirecting...</div>
    }
}