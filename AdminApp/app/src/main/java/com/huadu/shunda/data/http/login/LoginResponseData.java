package com.huadu.shunda.data.http.login;

import com.google.gson.annotations.SerializedName;

public class LoginResponseData {
    @SerializedName("userId")
    private int userId;

    @SerializedName("userName")
    private String userName;

    @SerializedName("canExpressQuery")
    private boolean canExpressQuery;

    @SerializedName("canExpressOnShelf")
    private boolean canExpressOnShelf;

    @SerializedName("canExpressWarehouseCheck")
    private boolean canExpressWarehouseCheck;

    @SerializedName("canExpressOffShelf")
    private boolean canExpressOffShelf;

    @SerializedName("canExpressReturnOffShelf")
    private boolean canExpressReturnOffShelf;

    @SerializedName("canExpressDump")
    private boolean canExpressDump;

    @SerializedName("canExpressPacking")
    private boolean canExpressPacking;

    @SerializedName("canPackBoxQuery")
    private boolean canPackBoxQuery;

    @SerializedName("canPackBoxShip")
    private boolean canPackBoxShip;

    @SerializedName("canPhotoSignIn")
    private boolean canPhotoSignIn;

    @SerializedName("canPhotoCheckIn")
    private boolean canPhotoCheckIn;

    @SerializedName("canPhotoIssue")
    private boolean canPhotoIssue;

    @SerializedName("canPhotoPack")
    private boolean canPhotoPack;

    @SerializedName("canPhotoReturn")
    private boolean canPhotoReturn;

    @SerializedName("canPhotoShip")
    private boolean canPhotoShip;

    @SerializedName("canPhoto")
    private boolean canPhoto;

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

    public boolean getCanExpressQuery() {
        return canExpressQuery;
    }

    public void setCanExpressQuery(boolean canExpressQuery) {
        this.canExpressQuery = canExpressQuery;
    }

    public boolean getCanExpressOnShelf() {
        return canExpressOnShelf;
    }

    public void setCanExpressOnShelf(boolean canExpressOnShelf) {
        this.canExpressOnShelf = canExpressOnShelf;
    }

    public boolean getCanExpressWarehouseCheck() {
        return canExpressWarehouseCheck;
    }

    public void setCanExpressWarehouseCheck(boolean canExpressWarehouseCheck) {
        this.canExpressWarehouseCheck = canExpressWarehouseCheck;
    }

    public boolean getCanExpressOffShelf() {
        return canExpressOffShelf;
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

    public boolean getCanExpressDump() {
        return canExpressDump;
    }

    public void setCanExpressDump(boolean canExpressDump) {
        this.canExpressDump = canExpressDump;
    }

    public boolean getCanExpressPacking() {
        return canExpressPacking;
    }

    public void setCanExpressPacking(boolean canExpressPacking) {
        this.canExpressPacking = canExpressPacking;
    }

    public boolean getCanPackBoxQuery() {
        return canPackBoxQuery;
    }

    public void setCanPackBoxQuery(boolean canPackBoxQuery) {
        this.canPackBoxQuery = canPackBoxQuery;
    }

    public boolean getCanPackBoxShip() {
        return canPackBoxShip;
    }

    public void setCanPackBoxShip(boolean canPackBoxShip) {
        this.canPackBoxShip = canPackBoxShip;
    }

    public boolean getCanPhotoSignIn() {
        return canPhotoSignIn;
    }

    public void setCanPhotoSignIn(boolean canPhotoSignIn) {
        this.canPhotoSignIn = canPhotoSignIn;
    }

    public boolean getCanPhotoCheckIn() {
        return canPhotoCheckIn;
    }

    public void setCanPhotoCheckIn(boolean canPhotoCheckIn) {
        this.canPhotoCheckIn = canPhotoCheckIn;
    }

    public boolean getCanPhotoIssue() {
        return canPhotoIssue;
    }

    public void setCanPhotoIssue(boolean canPhotoIssue) {
        this.canPhotoIssue = canPhotoIssue;
    }

    public boolean getCanPhotoPack() {
        return canPhotoPack;
    }

    public void setCanPhotoPack(boolean canPhotoPack) {
        this.canPhotoPack = canPhotoPack;
    }

    public boolean getCanPhotoReturn() {
        return canPhotoReturn;
    }

    public void setCanPhotoReturn(boolean canPhotoReturn) {
        this.canPhotoReturn = canPhotoReturn;
    }

    public boolean getCanPhotoShip() {
        return canPhotoShip;
    }

    public void setCanPhotoShip(boolean canPhotoShip) {
        this.canPhotoShip = canPhotoShip;
    }

    public boolean getCanPhoto() {
        return canPhoto;
    }

    public void setCanPhoto(boolean canPhoto) {
        this.canPhoto = canPhoto;
    }
}
