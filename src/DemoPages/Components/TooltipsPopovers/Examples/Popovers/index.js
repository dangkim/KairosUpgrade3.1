import React, { Fragment } from 'react';
import {
    CSSTransition,
    TransitionGroup,
} from 'react-transition-group';
import {
    Row, Col,
    Card, CardBody,
    CardTitle
} from 'reactstrap';

import PopoversBasicExample from './Basic';
import PopoversColorsExample from './Colors';
import PopoversGradientsExample from './Gradients';
import PopoversCustomExample from './Custom';

const PopoversExample = (props) => {
    return (
        <Fragment>
            <TransitionGroup component="div">
                <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                    <Row>
                        <Col lg="6">
                            <Card className="main-card mb-3">
                                <CardBody>
                                    <CardTitle>Popovers Alignments</CardTitle>
                                    <PopoversBasicExample />
                                </CardBody>
                            </Card>
                        </Col>
                        <Col lg="6">
                            <Card className="main-card mb-3">
                                <CardBody>
                                    <CardTitle>Solid Colors</CardTitle>
                                    <PopoversColorsExample />
                                </CardBody>
                            </Card>
                        </Col>
                    </Row>
                </CSSTransition>
                <CSSTransition timeout={1500} unmountOnExit appear classNames="TabsAnimation">
                    <Card className="main-card mb-3">
                        <CardBody>
                            <CardTitle>Gradient Colors</CardTitle>
                            <PopoversGradientsExample />
                        </CardBody>
                    </Card>
                </CSSTransition>
            </TransitionGroup>
        </Fragment>
    );
};

export default PopoversExample;
