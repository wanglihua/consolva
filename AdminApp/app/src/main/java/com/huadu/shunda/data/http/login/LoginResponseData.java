package com.huadu.shunda.data.http.login;

import com.google.gson.annotations.SerializedName;

public class LoginResponseData {
    @SerializedName("userId")
    private int userId;

    @SerializedName("userName")
    private String userName;

    @SerializedName("canPhotoSignIn")
    private boolean canPhotoSignIn;

    @SerializedName("canPhotoCheckIn")
    private boolean canPhotoCheckIn;

    @SerializedName("canPhotoIssue")
    private boolean canPhotoIssue;

    @SerializedName("canPhotoCnDamage")
    private boolean canPhotoCnDamage;

    @SerializedName("canPhotoReturn")
    private boolean canPhotoReturn;

    @SerializedName("canPhotoShip")
    private boolean canPhotoShip;

    @SerializedName("canPhotoAuDamage")
    private boolean canPhotoAuDamage;

    @SerializedName("canPhotoDeliver")
    private boolean canPhotoDeliver;

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

    public boolean getCanPhotoCnDamage() {
        return canPhotoCnDamage;
    }

    public void setCanPhotoCnDamage(boolean canPhotoCnDamage) {
        this.canPhotoCnDamage = canPhotoCnDamage;
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

    public boolean getCanPhotoAuDamage() {
        return canPhotoAuDamage;
    }

    public void setCanPhotoAuDamage(boolean canPhotoAuDamage) {
        this.canPhotoAuDamage = canPhotoAuDamage;
    }

    public boolean getCanPhotoDeliver() {
        return canPhotoDeliver;
    }

    public void setCanPhotoDeliver(boolean canPhotoDeliver) {
        this.canPhotoDeliver = canPhotoDeliver;
    }

    public boolean getCanPhoto() {
        return canPhoto;
    }

    public void setCanPhoto(boolean canPhoto) {
        this.canPhoto = canPhoto;
    }
}
