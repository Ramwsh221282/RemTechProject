import {ParserProfile} from "../../Types/ParserProfile.ts";
import {Modal} from "../../../../components/Modal.tsx";
import {Button} from "@mui/material";
import {CustomTabs} from "../../../../components/CustomTabPanel.tsx";

type Props = {
    isOpen: boolean;
    handleClose: () => void;
    profile: ParserProfile;
}

export function ParserProfileCardModal(props: Props) {
    return (
        <Modal isOpen={props.isOpen} onClose={() => {
        }} title={`Управление профилем: ${props.profile.name}`}
               actions={[<Button onClick={props.handleClose}>Закрыть и сохранить</Button>,
                   <Button onClick={props.handleClose}>Закрыть</Button>]}>
            <CustomTabs panels={[
                {
                    index: 0,
                    title: 'Ссылки',
                    children: <div>{"Ссылки"}</div>
                },
                {
                    index: 1,
                    title: 'Жизненный цикл',
                    children: <ParserProfileLifeManagement {...props.profile} />
                }
            ]}>

            </CustomTabs>
        </Modal>
    )
}

function ParserProfileLifeManagement(profile: ParserProfile) {
    return (
        <>
            {profile.state ? <Button color={"error"}>{"Деактивировать"}</Button> :
                <Button color={"success"}>{"Активировать"}</Button>}
            <Button color={"error"}>{"Удалить профиль"}</Button>
        </>
    )
}