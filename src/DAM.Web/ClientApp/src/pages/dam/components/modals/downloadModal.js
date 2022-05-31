import React, { memo } from 'react';
import { connect } from 'react-redux';
import { Modal, Button } from 'antd';
import { useTranslation } from 'react-i18next';

function DownloadModal(props) {
    const { t } = useTranslation();
    const { modal, checkedAssetsItem, downloadExt } = props;

    const handleSubmitClick = (e, showWatermark = false) => {
        modal().download(checkedAssetsItem, showWatermark, downloadExt);
        props.setIsDownloadModalOpen(false);
    }

    const handleCloseModal = () => {
        props.setIsDownloadModalOpen(false);
    }

    return (
        <Modal
            className="alert-modals"
            title={t('Download Asset.Title')}
            visible={props.isDownloadModalOpen}
            onCancel={handleCloseModal}
            closable={false}
            footer={[
                <Button type="secondary" onClick={handleCloseModal}>
                    {t('Button.Cancel')}
                </Button>,
                <Button type="secondary" onClick={(e) => handleSubmitClick(e, true)}>
                    {t('Button.Yes')}
                </Button>,
                <Button type="secondary" onClick={handleSubmitClick}>
                    {t('Button.No')}
                </Button>]
            }
        >
            <p>{t('Download Asset.Watermark')}</p>
        </Modal>
    );
}

function mapStateToProps(state) {
    return {};
}

function mapDispatchToProps(dispatch) {
    return {};
}

export default connect(mapStateToProps, mapDispatchToProps)(memo(DownloadModal));
