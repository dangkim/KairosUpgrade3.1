import React, { Component } from 'react';
import { connect } from 'react-redux';
import cx from 'classnames';
import {
    Row, Col, Card, CardBody
} from 'reactstrap';
class PageTitleCategory extends Component {

    render() {
        let {
            enablePageTitleIcon,
            heading,
            icon,
        } = this.props;

        return (

            <div className="app-page-title">
                <div className="page-title-wrapper">
                    <div className="page-title-heading">
                        {/* <div>
                            <CardBody>
                                <Row>

                                    <Col md="12" className={cx("font-icon-wrapper font-icon-lg", { 'd-none': !enablePageTitleIcon })}>

                                        <i className={icon} />

                                    </Col>
                                </Row>
                            </CardBody>
                        </div> */}

                        <div
                            className={cx("page-title-icon", { 'd-none': !enablePageTitleIcon })}>
                            <i className={icon} />
                        </div>
                        <div>
                            {heading}
                            {/* <div
                                className={cx("page-title-subheading", { 'd-none': !enablePageTitleSubheading })}>
                                {subheading}
                            </div> */}
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

const mapStateToProps = state => ({
    enablePageTitleIcon: state.theme.enablePageTitleIcon,
    enablePageTitleSubheading: state.theme.enablePageTitleSubheading,
});

const mapDispatchToProps = dispatch => ({});

export default connect(mapStateToProps, mapDispatchToProps)(PageTitleCategory);