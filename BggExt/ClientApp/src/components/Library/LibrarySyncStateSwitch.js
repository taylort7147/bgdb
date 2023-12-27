import React, { Component } from 'react';
import Switch from "react-switch";
import PropTypes from "prop-types";
import { getToken } from '../../useToken';


export class LibrarySyncStateSwitch extends Component {
    static displayName = LibrarySyncStateSwitch.name;

    constructor(props) {
        super(props);
        this.libraryId = props.libraryId;
        this.state = { checked: props.initialState ?? false };
        this.handleChange = this.handleChange.bind(this);
    }

    handleChange(checked) {
        // Assume the operation is successful
        this.setState({checked: checked});

        // Now make the API call and update the state
        fetch(`api/library/setsyncstate/${this.libraryId}`, {
            method: "POST",
            headers: new Headers({
                'Authorization': `Bearer ${getToken()?.accessToken}`,
                'Content-Type': "application/json"
            }),
            body: checked
        })
            .then(response => response.json())
            .then(data => {
                if (typeof (data) == "boolean") {
                    this.setState({ checked: data });
                }
            });
    }

    render() {
        return (
            <Switch onChange={this.handleChange} checked={this.state.checked} />
        );
    }
}

LibrarySyncStateSwitch.propTypes = {
    libraryId: PropTypes.string.isRequired,
    initialState: PropTypes.bool
};
