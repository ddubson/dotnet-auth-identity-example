import React, {PureComponent} from 'react';

export class Home extends PureComponent {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.performLogin = this.performLogin.bind(this);

        this.state = {
            isLoggedIn: false,
            userName: null,
        }
    }

    componentDidMount() {
        this.props.openIdManager.getUser().then((user) => {
            if (user) {
                this.setState({isLoggedIn: true, userName: user.profile.preferred_username});
            } else {    
                this.setState({isLoggedIn: false, userName: null});
            }
        });
    }

    render() {
        return (
            <div>
                <button id="login" onClick={this.performLogin}>Login</button>
                <button id="api">Call API</button>
                <button id="logout">Logout</button>

                <div className="info">{this.state.isLoggedIn ? <div>{this.state.userProfile}</div> : "User is not logged in."}</div>
                
                <pre id="results"></pre>
            </div>
        );
    }

    performLogin() {
        this.props.openIdManager.signinRedirect();
    }
}
