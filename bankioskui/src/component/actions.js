import React, { Fragment } from 'react';
import Button from 'react-bootstrap/Button'
import { Loader } from 'react-overlay-loader';
import ApiCalls from '../helpers/ApiCalls';
import Audit from '../helpers/Audit';

import './../css/index.css';

class ActionLists extends React.Component {
    
    onClick = (event) => {
        let action = event.target.getAttribute('action');
        // Cascade that up
        this.props.onActionSelected(action);
    }

    render() {
        if (!this.props.show) {
            return ("");
        }
        else {
            return (
                <Fragment>
                    <div className="text-center mt-2">
                        <h5>
                            Please select the action you want to do today. This will help us optimize your experience.
                        </h5>
                        <div className="d-flex flex-wrap align-items-stretch">
                            {this.props.actionsList.map((action) => 
                                <div className="w-50 p-2" key={action}>
                                    <Button className="mt-2 p-2 w-75 h-100 btn-lg" onClick={this.onClick} key={action} action={action}>{action}</Button>
                                </div>
                            )}  
                        </div>
                    </div>
                </Fragment>
            );
        }
    }
}

class ActionRegistered extends React.Component {
    render () {
        if (!this.props.show) {
            return ("");
        }
        else {
            return (
                <Fragment>
                    <div className="text-center mt-5">
                        <h4>
                            Thank you - your request has been sent to our team. They will get back to you shortly.
                        </h4>
                    </div>
                </Fragment>
            );
        }
    }
}

class Actions extends React.Component {

    constructor(props) {
        super(props)
        this.state = {
            showLoadingOverlay: true,
            actionsList: [],
            step: "actionslist"
        }
    }

    componentDidMount() {
        this.loadActions();
    }

    loadActions = () => {
        let api = new ApiCalls();
        api.Get(api.actionsListEndPoint())
            .then(response => response.json())
            .then(data => {
                if (data.status) {
                    let actionsList = data.value.map(a => {return a;});
                    this.setState({
                        showLoadingOverlay: false,
                        actionsList: actionsList
                    });
                }
            });
    }

    onActionSelected = (action) => {
        // Add that to the log
        this.setState({ showLoadingOverlay: true }, () => {
            let audit = new Audit();
            audit.ActionPerformed(action)
                .then(res => {
                    this.setState({
                        showLoadingOverlay: false,
                        step: "actionregistered"
                    });
                });
        });
    }

    render() {
        return (
            <Fragment>
                <Loader fullPage loading={this.state.showLoadingOverlay} containerStyle={{background: "rgba(255, 255, 255, 0.9)"}}/>
                <ActionLists actionsList={this.state.actionsList} onActionSelected={this.onActionSelected} show={this.state.step === "actionslist"} />
                <ActionRegistered show={this.state.step === "actionregistered"} />
            </Fragment>
        );
    }
}


export default Actions;