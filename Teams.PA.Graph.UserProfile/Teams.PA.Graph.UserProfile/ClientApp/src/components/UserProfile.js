import React, { Component } from "react";

import * as microsoftTeams from "@microsoft/teams-js";
import Axios from "axios";

export class UserProfile extends Component {
  static displayName = UserProfile.name;

  constructor(props) {
    super(props);
    this.state = { userInfo: [], loading: true };
  }

  componentDidMount() {
    microsoftTeams.initialize();
    microsoftTeams.getContext((context) => {
      microsoftTeams.authentication.getAuthToken({
        successCallback: (token) => {
          this.populateUserProfileInfo(token);
          microsoftTeams.appInitialization.notifySuccess();
        },
        failureCallback: (error) => {
          microsoftTeams.appInitialization.notifyFailure({
            reason: microsoftTeams.appInitialization.FailedReason.AuthFailed,
            error,
          });
        },
        resources: [
          "api://ehteamsapp.com.ngrok.io/454c6221-4c94-4582-9846-da009415ab8e",
        ],
      });
    });
  }

  static renderUserInfo(user) {
    debugger;
    return (
      <>
        <ul className="list-group">
          <li className="list-group-item active">{user.displayName}</li>
          <li className="list-group-item">{user.jobTitle}</li>
          <li className="list-group-item">{user.email}</li>
          <li className="list-group-item">{user.officeLocation}</li>
          <li className="list-group-item">{user.mobilePhone}</li>
        </ul>
      </>
    );
  }

  render() {
    let contents = this.state.loading ? (
      <p>
        <em>Loading...</em>
      </p>
    ) : (
      UserProfile.renderUserInfo(this.state.userInfo)
    );

    return (
      <div>
        <h1 id="tabelLabel">User Profile Info</h1>
        <p>
          This component demonstrates fetching data from the Microsoft Graph
          using Single Sign On approach.
        </p>
        {contents}
      </div>
    );
  }

  async populateUserProfileInfo(token) {
    const response = await Axios.get("/api/user/GetUserProfile", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    const data = response.data;
    debugger;
    this.setState({ userInfo: data, loading: false });
  }
}
