import React, {PureComponent} from 'react';
import * as axios from "axios";

export class Home extends PureComponent {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.performLogin = this.performLogin.bind(this);
        this.performLogout = this.performLogout.bind(this);
        this.renderUserDetails = this.renderUserDetails.bind(this);

        this.state = {
            isLoggedIn: false,
            userName: null,
            produceItems: []
        }
    }

    componentDidMount() {
        this.props.openIdManager.getUser().then((user) => {
            if (user) {
                this.setState({isLoggedIn: true, userName: user.profile.preferred_username});
                axios.get('/api/values', {
                    baseURL: 'https://localhost:5005',
                    headers: {'Authorization': 'Bearer ' + user.access_token}
                }).then((response) => {
                    this.setState({produceItems: response.data})
                }).catch(e => console.error(e))
            } else {
                this.setState({isLoggedIn: false, userName: null, produceItems: []});
            }
        });
    }

    render() {
        return (
            <div>
                {!this.state.isLoggedIn && <button id="login" onClick={this.performLogin}>Login</button>}

                {this.state.isLoggedIn && <button id="logout" onClick={this.performLogout}>Logout</button>}

                <div
                    className="info">{this.state.isLoggedIn ? this.renderUserDetails() : "User is not logged in."}</div>

                <pre id="results"></pre>
            </div>
        );
    }

    performLogin() {
        this.props.openIdManager.signinRedirect();
    }

    performLogout() {
        this.props.openIdManager.signoutRedirect();
    }

    renderUserDetails() {
        return (
            <React.Fragment>
                <div>User is logged in as:</div>
                <div>{this.state.userName}</div>
                <hr/>

                <div>(Auth Protected Resources)</div>
                <div>My Produce Items</div>
                {this.state.produceItems.map((produceItem) => {
                    return <div>{produceItem}</div>
                })}
            </React.Fragment>
        )
    }
}
