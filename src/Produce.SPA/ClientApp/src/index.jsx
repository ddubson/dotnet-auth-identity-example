import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import registerServiceWorker from './registerServiceWorker';
import * as Oidc from "oidc-client";

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const config = {
    authority: "https://localhost:5000",
    client_id: "produce-spa",
    redirect_uri: "https://localhost:5010/callback",
    response_type: "code",
    scope:"openid profile ProduceAPI",
    post_logout_redirect_uri : "https://localhost:5010",
};

const openIdManager = new Oidc.UserManager(config);

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
    <App openIdManager={openIdManager} />
  </BrowserRouter>,
  rootElement);

registerServiceWorker();
