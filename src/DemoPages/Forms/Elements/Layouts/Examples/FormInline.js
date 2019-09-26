import React, { Fragment } from 'react';
import { Button, Form, FormGroup, Label, Input } from 'reactstrap';
import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';

export default class FormInline extends React.Component {
    render() {
        return (
            <Fragment>
                <TransitionGroup component="div">
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <Form inline>
                            <FormGroup className="mb-2 mr-sm-2 mb-sm-0">
                                <Label for="exampleEmail22" className="mr-sm-2">Email</Label>
                                <Input type="email" name="email" id="exampleEmail22" placeholder="something@idk.cool" />
                            </FormGroup>
                            <FormGroup className="mb-2 mr-sm-2 mb-sm-0">
                                <Label for="examplePassword22" className="mr-sm-2">Password</Label>
                                <Input type="password" name="password" id="examplePassword22" placeholder="don't tell!" />
                            </FormGroup>
                            <Button color="primary">Submit</Button>
                        </Form>
                    </CSSTransition>
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <div className="divider" />
                    </CSSTransition>
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <Form>
                            <FormGroup check inline>
                                <Label check>
                                    <Input type="checkbox" /> Some input
                            </Label>
                            </FormGroup>
                            <FormGroup check inline>
                                <Label check>
                                    <Input type="checkbox" /> Some other input
                            </Label>
                            </FormGroup>
                        </Form>
                    </CSSTransition>
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <div className="divider" />
                    </CSSTransition>
                    <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                        <Form inline>
                            <FormGroup>
                                <Label for="exampleEmail33" hidden>Email</Label>
                                <Input type="email" name="email" className="mr-2" id="exampleEmail33" placeholder="Email" />
                            </FormGroup>
                            {' '}
                            <FormGroup>
                                <Label for="examplePassword44" hidden>Password</Label>
                                <Input type="password" className="mr-2" name="password" id="examplePassword44"
                                    placeholder="Password" />
                            </FormGroup>
                            {' '}
                            <Button color="primary">Submit</Button>
                        </Form>
                    </CSSTransition>
                </TransitionGroup>
            </Fragment>
        );
    }
}
