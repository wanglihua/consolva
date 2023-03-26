package com.huadu.shunda.data.http.login;

import com.google.gson.annotations.SerializedName;
import com.huadu.shunda.data.http.ResponseInfo;

public class LoginResponseInfo extends ResponseInfo {

    @SerializedName("userId")
    private int userId;

    @SerializedName("userName")
    private String userName;

    @SerializedName("token")
    private String token;

    @SerializedName("canExpressOnShelf")
    private boolean canExpressOnShelf;

    @SerializedName("canExpressWarehouseCheck")
    private boolean canExpressWarehouseCheck;

    @SerializedName("canExpressOffShelf")
    private boolean canExpressOffShelf;

    @SerializedName("canExpressReturnOffShelf")
    private boolean canExpressReturnOffShelf;

    @SerializedName("canExpressPacking")
    private boolean canExpressPacking;

    @SerializedName("canExpressDump")
    private boolean canExpressDump;

    @SerializedName("canLoad")
    private boolean canLoad;

    @SerializedName("canUnload")
    private boolean canUnload;

    @SerializedName("canUnloadSort")
    private boolean canUnloadSort;

    @SerializedName("canWarehousePickupBooking")
    private boolean canWarehousePickupBooking;

    @SerializedName("canWarehousePickup")
    private boolean canWarehousePickup;

    @SerializedName("canPackBoxDetail")
    private boolean canPackBoxDetail;

    @SerializedName("canPackBoxStockPlace")
    private boolean canPackBoxStockPlace;

    @SerializedName("canCustomerPackBoxStockPlace")
    private boolean canCustomerPackBoxStockPlace;

    @SerializedName("canOrderPayDetail")
    private boolean canOrderPayDetail;

    @SerializedName("canStockCheck")
    private boolean canStockCheck;

    @SerializedName("canStockMove")
    private boolean canStockMove;

    @SerializedName("canExpressPackageQuery")
    private boolean canExpressPackageQuery;

    @SerializedName("canTerminalHandover")
    private boolean canTerminalHandover;

    @SerializedName("canTerminalPickup")
    private boolean canTerminalPickup;

    @SerializedName("canTerminalPickupBooking")
    private boolean canTerminalPickupBooking;

    @SerializedName("canHomeDeliver")
    private boolean canHomeDeliver;

    @SerializedName("canHomeDeliverBatch")
    private boolean canHomeDeliverBatch;

    @SerializedName("canLogisticsHomeDeliver")
    private boolean canLogisticsHomeDeliver;

    @SerializedName("canOrderPhotoSignIn")
    private boolean canOrderPhotoSignIn;

    @SerializedName("canOrderPhotoCheckIn")
    private boolean canOrderPhotoCheckIn;

    @SerializedName("canOrderPhotoIssue")
    private boolean canOrderPhotoIssue;

    @SerializedName("canOrderPhotoCnDamage")
    private boolean canOrderPhotoCnDamage;

    @SerializedName("canOrderPhotoReturn")
    private boolean canOrderPhotoReturn;

    @SerializedName("canOrderPhotoShip")
    private boolean canOrderPhotoShip;

    @SerializedName("canOrderPhotoAuDamage")
    private boolean canOrderPhotoAuDamage;

    @SerializedName("canOrderPhotoDeliver")
    private boolean canOrderPhotoDeliver;

    @SerializedName("canOrderPhoto")
    private boolean canOrderPhoto;

    @SerializedName("canPackBoxLabelPrint")
    private boolean canPackBoxLabelPrint;

    @SerializedName("canPackBoxLabelTlaPrint")
    private boolean canPackBoxLabelTlaPrint;

    public int getUserId() {
        return userId;
    }

    public void setUserId(int userId) {
        this.userId = userId;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public boolean getCanExpressOnShelf() {
        return canExpressOnShelf;
    }

    public void setCanExpressOnShelf(boolean canExpressOnShelf) {
        this.canExpressOnShelf = canExpressOnShelf;
    }

    public boolean getCanExpressOffShelf() {
        return canExpressOffShelf;
    }

    public boolean getCanExpressWarehouseCheck() {
        return canExpressWarehouseCheck;
    }

    public void setCanExpressWarehouseCheck(boolean canExpressWarehouseCheck) {
        this.canExpressWarehouseCheck = canExpressWarehouseCheck;
    }

    public void setCanExpressOffShelf(boolean canExpressOffShelf) {
        this.canExpressOffShelf = canExpressOffShelf;
    }

    public boolean getCanExpressReturnOffShelf() {
        return canExpressReturnOffShelf;
    }

    public void setCanExpressReturnOffShelf(boolean canExpressReturnOffShelf) {
        this.canExpressReturnOffShelf = canExpressReturnOffShelf;
    }

    public boolean getCanExpressPacking() {
        return canExpressPacking;
    }

    public void setCanExpressPacking(boolean canExpressPacking) {
        this.canExpressPacking = canExpressPacking;
    }

    public boolean getCanExpressDump() {
        return canExpressDump;
    }

    public void setCanExpressDump(boolean canExpressDump) {
        this.canExpressDump = canExpressDump;
    }

    public boolean getCanLoad() {
        return canLoad;
    }

    public void setCanLoad(boolean canLoad) {
        this.canLoad = canLoad;
    }

    public boolean getCanUnload() {
        return canUnload;
    }

    public void setCanUnload(boolean canUnload) {
        this.canUnload = canUnload;
    }

    public boolean getCanUnloadSort() {
        return canUnloadSort;
    }

    public void setCanUnloadSort(boolean canUnloadSort) {
        this.canUnloadSort = canUnloadSort;
    }

    public boolean getCanWarehousePickup() {
        return canWarehousePickup;
    }

    public boolean getCanWarehousePickupBooking() {
        return canWarehousePickupBooking;
    }

    public void setCanWarehousePickupBooking(boolean canWarehousePickupBooking) {
        this.canWarehousePickupBooking = canWarehousePickupBooking;
    }

    public void setCanWarehousePickup(boolean canWarehousePickup) {
        this.canWarehousePickup = canWarehousePickup;
    }

    public boolean getCanPackBoxDetail() {
        return canPackBoxDetail;
    }

    public void setCanPackBoxDetail(boolean canPackBoxDetail) {
        this.canPackBoxDetail = canPackBoxDetail;
    }

    public boolean getCanPackBoxStockPlace() {
        return canPackBoxStockPlace;
    }

    public void setCanPackBoxStockPlace(boolean canPackBoxStockPlace) {
        this.canPackBoxStockPlace = canPackBoxStockPlace;
    }

    public boolean getCanCustomerPackBoxStockPlace() {
        return canCustomerPackBoxStockPlace;
    }

    public void setCanCustomerPackBoxStockPlace(boolean canCustomerPackBoxStockPlace) {
        this.canCustomerPackBoxStockPlace = canCustomerPackBoxStockPlace;
    }

    public boolean getCanOrderPayDetail() {
        return canOrderPayDetail;
    }

    public void setCanOrderPayDetail(boolean canOrderPayDetail) {
        this.canOrderPayDetail = canOrderPayDetail;
    }

    public boolean getCanStockCheck() {
        return canStockCheck;
    }

    public void setCanStockCheck(boolean canStockCheck) {
        this.canStockCheck = canStockCheck;
    }

    public boolean getCanStockMove() {
        return canStockMove;
    }

    public void setCanStockMove(boolean canStockMove) {
        this.canStockMove = canStockMove;
    }

    public boolean getCanExpressPackageQuery() {
        return canExpressPackageQuery;
    }

    public void setCanExpressPackageQuery(boolean canExpressPackageQuery) {
        this.canExpressPackageQuery = canExpressPackageQuery;
    }

    public boolean getCanTerminalHandover() {
        return canTerminalHandover;
    }

    public void setCanTerminalHandover(boolean canTerminalHandover) {
        this.canTerminalHandover = canTerminalHandover;
    }

    public boolean getCanTerminalPickupBooking() {
        return canTerminalPickupBooking;
    }

    public void setCanTerminalPickupBooking(boolean canTerminalPickupBooking) {
        this.canTerminalPickupBooking = canTerminalPickupBooking;
    }

    public boolean getCanTerminalPickup() {
        return canTerminalPickup;
    }

    public void setCanTerminalPickup(boolean canTerminalPickup) {
        this.canTerminalPickup = canTerminalPickup;
    }

    public boolean getCanHomeDeliver() {
        return canHomeDeliver;
    }

    public void setCanHomeDeliver(boolean canHomeDeliver) {
        this.canHomeDeliver = canHomeDeliver;
    }

    public boolean getCanHomeDeliverBatch() {
        return canHomeDeliverBatch;
    }

    public void setCanHomeDeliverBatch(boolean canHomeDeliverBatch) {
        this.canHomeDeliverBatch = canHomeDeliverBatch;
    }

    public boolean getCanLogisticsHomeDeliver() {
        return canLogisticsHomeDeliver;
    }

    public void setCanLogisticsHomeDeliver(boolean canLogisticsHomeDeliver) {
        this.canLogisticsHomeDeliver = canLogisticsHomeDeliver;
    }

    public boolean getCanOrderPhotoSignIn() {
        return canOrderPhotoSignIn;
    }

    public void setCanOrderPhotoSignIn(boolean canOrderPhotoSignIn) {
        this.canOrderPhotoSignIn = canOrderPhotoSignIn;
    }

    public boolean getCanOrderPhotoCheckIn() {
        return canOrderPhotoCheckIn;
    }

    public void setCanOrderPhotoCheckIn(boolean canOrderPhotoCheckIn) {
        this.canOrderPhotoCheckIn = canOrderPhotoCheckIn;
    }

    public boolean getCanOrderPhotoIssue() {
        return canOrderPhotoIssue;
    }

    public void setCanOrderPhotoIssue(boolean canOrderPhotoIssue) {
        this.canOrderPhotoIssue = canOrderPhotoIssue;
    }

    public boolean getCanOrderPhotoCnDamage() {
        return canOrderPhotoCnDamage;
    }

    public void setCanOrderPhotoCnDamage(boolean canOrderPhotoCnDamage) {
        this.canOrderPhotoCnDamage = canOrderPhotoCnDamage;
    }

    public boolean getCanOrderPhotoReturn() {
        return canOrderPhotoReturn;
    }

    public void setCanOrderPhotoReturn(boolean canOrderPhotoReturn) {
        this.canOrderPhotoReturn = canOrderPhotoReturn;
    }

    public boolean getCanOrderPhotoShip() {
        return canOrderPhotoShip;
    }

    public void setCanOrderPhotoShip(boolean canOrderPhotoShip) {
        this.canOrderPhotoShip = canOrderPhotoShip;
    }

    public boolean getCanOrderPhotoAuDamage() {
        return canOrderPhotoAuDamage;
    }

    public void setCanOrderPhotoAuDamage(boolean canOrderPhotoAuDamage) {
        this.canOrderPhotoAuDamage = canOrderPhotoAuDamage;
    }

    public boolean getCanOrderPhotoDeliver() {
        return canOrderPhotoDeliver;
    }

    public void setCanOrderPhotoDeliver(boolean canOrderPhotoDeliver) {
        this.canOrderPhotoDeliver = canOrderPhotoDeliver;
    }

    public boolean getCanOrderPhoto() {
        return canOrderPhoto;
    }

    public void setCanOrderPhoto(boolean canOrderPhoto) {
        this.canOrderPhoto = canOrderPhoto;
    }

    public boolean getCanPackBoxLabelPrint() {
        return canPackBoxLabelPrint;
    }

    public void setCanPackBoxLabelPrint(boolean canPackBoxLabelPrint) {
        this.canPackBoxLabelPrint = canPackBoxLabelPrint;
    }

    public boolean getCanPackBoxLabelTlaPrint() {
        return canPackBoxLabelTlaPrint;
    }

    public void setCanPackBoxLabelTlaPrint(boolean canPackBoxLabelTlaPrint) {
        this.canPackBoxLabelTlaPrint = canPackBoxLabelTlaPrint;
    }
}
